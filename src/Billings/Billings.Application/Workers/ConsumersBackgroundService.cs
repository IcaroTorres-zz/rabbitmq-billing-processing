using Billings.Application.Usecases;
using Billings.Domain.Models;
using Library.Messaging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Billings.Application.Workers
{
    public class ConsumersBackgroundService : BackgroundService
    {
        private readonly IMessageConsumer<ProcessedBatch> _processedBatchConsumer;

        public ConsumersBackgroundService(IMessageConsumer<ProcessedBatch> processedBatchConsumer)
        {
            _processedBatchConsumer = processedBatchConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processedBatchConsumer.ConsumeWithUsecase(nameof(ConfirmProcessedBatchUsecase));
        }
    }
}
