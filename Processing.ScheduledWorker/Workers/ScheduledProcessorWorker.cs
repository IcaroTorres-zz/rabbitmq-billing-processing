using Library.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Processing.ScheduledWorker.Domain.Models;
using Processing.ScheduledWorker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.ScheduledWorker.Workers
{
    public class ScheduledProcessorWorker : BackgroundService
    {
        private readonly IRpcClient<List<Customer>> _customerClient;
        private readonly IRpcClient<List<Billing>> _billingClient;
        private readonly IAmountProcessor _processor;
        private readonly IComparer<ICpfCarrier> _comparer;
        private readonly ScheduledProcessorSettings _config;
        private readonly ILogger<ScheduledProcessorWorker> _logger;

        public ScheduledProcessorWorker(
            IRpcClient<List<Customer>> customerClient,
            IRpcClient<List<Billing>> billingClient,
            IAmountProcessor processor,
            IComparer<ICpfCarrier> comparer,
            ScheduledProcessorSettings config,
            ILogger<ScheduledProcessorWorker> logger)
        {
            _customerClient = customerClient;
            _billingClient = billingClient;
            _processor = processor;
            _comparer = comparer;
            _config = config;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{DateTime.UtcNow:G} Starting scheduled batch processing ...");
            var batch = new ProcessBatch();
            while (true)
            {
                try
                {
                    batch = await DoExecute(batch);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"{DateTime.UtcNow:G} BatchId: {batch.Id}, Exceptions: {string.Join(Environment.NewLine, ex.ExtractMessages())}");
                }
            }
        }

        internal async Task<ProcessBatch> DoExecute(ProcessBatch batch)
        {
            batch = await FetchBatchAsync(batch);
            batch = ProcessBatch(batch);
            _logger.LogInformation($"{DateTime.UtcNow:G} BatchId: {batch.Id}. Waiting {_config.MillisecondsScheduledTime} milliseconds to process next batch...");
            return await batch.ResetIdAfter(_config.MillisecondsScheduledTime);
        }

        internal async Task<ProcessBatch> FetchBatchAsync(ProcessBatch batch)
        {
            var billingTask = Task.Run(() =>
            {
                batch.Billings = _billingClient.CallProcedure(batch.Billings);
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batch.Id}. Billings ready to process...");
            });
            var customerTask = Task.Run(() =>
            {
                batch.Customers = new List<ICpfCarrier>(_customerClient.CallProcedure(""));
                batch.Customers.Sort(_comparer);
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batch.Id}. Customers ready to process...");
            });

            await Task.WhenAll(billingTask, customerTask);
            return batch;
        }

        internal ProcessBatch ProcessBatch(ProcessBatch batch)
        {
            if (batch.Customers.Count == 0 || batch.Billings.Count == 0)
            {
                _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batch.Id}. Skiping batch. Nothing to process now...");
                return batch;
            }

            _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batch.Id}. Process started...");
            Parallel.For(0, batch.Billings.Count, billindIndex =>
            {
                var billing = batch.Billings[billindIndex];
                var customerIndex = batch.Customers.BinarySearch(billing, _comparer);
                if (customerIndex >= 0)
                {
                    var customer = batch.Customers[customerIndex];
                    batch.Billings[billindIndex] = _processor.Process(customer, billing);
                }
            });
            _logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batch.Id}. Process finished...");

            return batch;
        }
    }
}
