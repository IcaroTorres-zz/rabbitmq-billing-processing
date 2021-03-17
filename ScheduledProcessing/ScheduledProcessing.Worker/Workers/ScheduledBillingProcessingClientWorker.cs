using Library.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ScheduledProcessing.Worker.Domain.Models;
using ScheduledProcessing.Worker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduledProcessing.Worker.Workers
{
    public class ScheduledBillingProcessingClientWorker : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IAmountProcessor processor;
        private readonly IRpcClient<List<Customer>> customerClient;
        private readonly IRpcClient<List<Billing>> billingClient;
        private readonly ILogger<ScheduledBillingProcessingClientWorker> logger;
        private readonly int millisecondsScheduledTime;

        public ScheduledBillingProcessingClientWorker(
            IConnection connection,
            IAmountProcessor processor,
            IRpcClient<List<Customer>> customerClient,
            IRpcClient<List<Billing>> billingClient,
            ILogger<ScheduledBillingProcessingClientWorker> logger,
            IConfiguration config)
        {
            this.connection = connection;
            this.processor = processor;
            this.customerClient = customerClient;
            this.billingClient = billingClient;
            this.logger = logger;
            millisecondsScheduledTime = config.GetSection("MillisecondsScheduledTime").Get<int>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{DateTime.UtcNow:G} Starting scheduled batch processing ...");
            var customerComparer = new CustomerCpfComparer();
            var customerComparable = new Customer();
            var batchPayload = (new List<Billing>(), nameof(ScheduledBillingProcessingClientWorker));
            var batchId = Guid.NewGuid().ToString();
            try
            {
                while (true)
                {
                    var batchdata = await FetchBatchDataAsync(batchPayload, batchId);
                    var (billingPayload, customerPayload) = batchPayload;
                    billingPayload.Clear();
                    billingPayload = ProcessBatch(billingPayload, customerComparer, customerComparable, batchdata, batchId);
                    batchId = await WaitTillNextBatch(millisecondsScheduledTime);
                }
            }
            catch (Exception ex)
            {
                // Todo tratar melhor os erros
                logger.LogInformation($"{DateTime.UtcNow:G} BatchId: {batchId}, Exceptions: {string.Join(Environment.NewLine, ex.ExtractMessages())}");
                connection.Close();
            }
        }

        private async Task<(List<Customer>, List<Billing>)> FetchBatchDataAsync((List<Billing>, string) batchPayload, string batchId)
        {
            var (billingPayload, customerPayload) = batchPayload;
            var customerComparable = new Customer();
            var receivedBillings = new List<Billing>();
            var receivedCustomers = new List<Customer>();
            var billingTask = Task.Run(() =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                receivedBillings = billingClient.CallProcedure(billingPayload);
                stopWatch.Stop();
                logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Billings ready to process. Elapsed milliseconds {stopWatch.ElapsedMilliseconds}...");
            });
            var customerTask = Task.Run(() =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                receivedCustomers = customerClient.CallProcedure(customerPayload);
                stopWatch.Stop();
                logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Customers ready to process. Elapsed milliseconds {stopWatch.ElapsedMilliseconds}...");
            });

            await Task.WhenAll(billingTask, customerTask);
            return (receivedCustomers, receivedBillings);
        }

        private List<Billing> ProcessBatch(List<Billing> accumulator, CustomerCpfComparer customerComparer, Customer customerComparable, (List<Customer>, List<Billing>) batchdata, string batchId)
        {
            var (receivedCustomers, receivedBillings) = batchdata;

            if (receivedCustomers.Count == 0 || receivedBillings.Count == 0)
            {
                logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Skiping batch. Nothing to process now...");
            }
            else
            {
                logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Process started...");
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                Parallel.ForEach(receivedBillings, billing =>
                {
                    customerComparable.Cpf = billing.Cpf;
                    var customerIdex = receivedCustomers.BinarySearch(customerComparable, customerComparer);
                    var customer = receivedCustomers[customerIdex];
                    billing = processor.Process(customer, billing);
                    accumulator.Add(billing);
                });
                stopWatch.Stop();
                logger.LogInformation($"{DateTime.UtcNow:G}  BatchId: {batchId}. Process finished. Elapsed milliseconds {stopWatch.ElapsedMilliseconds}...");
            }

            return accumulator;
        }

        private async Task<string> WaitTillNextBatch(int millisecondsScheduledTime)
        {
            logger.LogInformation($"{DateTime.UtcNow:G}  Waiting {millisecondsScheduledTime} milliseconds to process next batch...");
            await Task.Delay(millisecondsScheduledTime);
            return Guid.NewGuid().ToString();
        }
    }
}
