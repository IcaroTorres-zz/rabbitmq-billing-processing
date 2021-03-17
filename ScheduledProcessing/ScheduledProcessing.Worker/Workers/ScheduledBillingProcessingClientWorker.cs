using Library.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ScheduledProcessing.Worker.Domain.Models;
using ScheduledProcessing.Worker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduledProcessing.Worker.Workers
{
    public class ScheduledBillingProcessingClientWorker : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IAmountProcessor processor;
        private readonly IRpcClient<HashSet<Customer>> customerClient;
        private readonly IRpcClient<HashSet<Billing>> billingClient;
        private readonly ILogger<ScheduledBillingProcessingClientWorker> logger;
        private readonly int millisecondsScheduledTime;

        public ScheduledBillingProcessingClientWorker(
            IConnection connection,
            IAmountProcessor processor,
            IRpcClient<HashSet<Customer>> customerClient,
            IRpcClient<HashSet<Billing>> billingClient,
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
            var serviceName = $"Service: {nameof(ScheduledBillingProcessingClientWorker)}";

            logger.LogInformation($"{DateTime.UtcNow:G} Starting {serviceName}...");
            var billingMessage = new HashSet<Billing>();
            var batchId = Guid.NewGuid().ToString();
            try
            {
                while (true)
                {
                    var receivedBillings = new HashSet<Billing>();
                    var receivedCustomers = new HashSet<Customer>();
                    var billingTask = Task.Run(() =>
                    {
                        receivedBillings = billingClient.CallProcedure(billingMessage);
                        logger.LogInformation($"{DateTime.UtcNow:G} {serviceName} BatchId: {batchId}. Billing ready to process...");
                    });
                    var customerTask = Task.Run(() =>
                    {
                        receivedCustomers = customerClient.CallProcedure(serviceName);
                        logger.LogInformation($"{DateTime.UtcNow:G} {serviceName} BatchId: {batchId}. Customers ready to process...");
                    });

                    await Task.WhenAll(billingTask, customerTask);
                    billingMessage.Clear();

                    if (receivedCustomers.Count == 0 || receivedBillings.Count == 0)
                    {
                        logger.LogInformation($"{DateTime.UtcNow:G} {serviceName} BatchId: {batchId}. Skiping batch. Nothing to process now...");
                    }
                    else
                    {
                        logger.LogInformation($"{DateTime.UtcNow:G} {serviceName} BatchId: {batchId}. Process started...");
                        Parallel.ForEach(receivedCustomers, customer =>
                        {
                            Parallel.ForEach(receivedBillings, billing =>
                            {
                                billing = processor.Process(customer, billing);
                                billingMessage.Add(billing);

                                logger.LogInformation($"Billing {billing.Id} processed at {billing.ProcessedAt:G}, for {customer.Cpf}... Amount {billing.Amount}");
                            });
                        });
                        logger.LogInformation($"{DateTime.UtcNow:G} {serviceName} BatchId: {batchId}. Process finished...");
                    }

                    logger.LogInformation($"{DateTime.UtcNow:G} {serviceName} Waiting {millisecondsScheduledTime} milliseconds to process next batch...");
                    await Task.Delay(millisecondsScheduledTime);
                    batchId = Guid.NewGuid().ToString();
                }
            }
            catch (Exception ex)
            {
                // Todo tratar melhor os erros
                logger.LogInformation($"{DateTime.UtcNow:G} BatchId: {batchId}, Exceptions: {string.Join(Environment.NewLine, ex.ExtractMessages())}");
                connection.Close();
            }
        }
    }
}
