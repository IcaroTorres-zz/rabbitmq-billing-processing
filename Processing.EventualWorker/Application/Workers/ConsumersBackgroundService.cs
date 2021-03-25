using Library.Messaging;
using Microsoft.Extensions.Hosting;
using Processing.EventualWorker.Application.Usecases;
using Processing.EventualWorker.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.EventualWorker.Application.Workers
{
    public class ConsumersBackgroundService : BackgroundService
    {
        private readonly IMessageConsumer<Billing> _billingIssuedConsumer;
        private readonly IMessageConsumer<ProcessedBatch> _batchProcessedConsumer;

        public ConsumersBackgroundService(IMessageConsumer<Billing> billingIssuedConsumer, IMessageConsumer<ProcessedBatch> batchConfirmedConsumer)
        {
            _billingIssuedConsumer = billingIssuedConsumer;
            _batchProcessedConsumer = batchConfirmedConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.WhenAll(
                _billingIssuedConsumer.ConsumeWithUsecase(nameof(HandleBillingIssuedUsecase)),
                _batchProcessedConsumer.ConsumeWithUsecase(nameof(HandleBatchConfirmedUsecase)));
        }
    }
}
