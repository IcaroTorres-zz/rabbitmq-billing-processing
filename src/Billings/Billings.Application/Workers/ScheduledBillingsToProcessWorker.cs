using Billings.Application.Abstractions;
using Billings.Domain.Models;
using Library.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Billings.Application.Workers
{
    public class ScheduledBillingsToProcessWorker : RpcServer<ScheduledBillingsToProcessWorker>
    {
        private readonly IBillingRepository _repository;

        public ScheduledBillingsToProcessWorker(IConnectionFactory factory, IBillingRepository repository, ILogger<ScheduledBillingsToProcessWorker> logger) : base(nameof(Billing), factory, logger)
        {
            _repository = repository;
        }

        public override async Task<string> HandleReceivedMessage(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var processedBatchReceivedMessage = await HandleProcessedBatchMessage(body);
            return processedBatchReceivedMessage;
        }

        public override async Task<string> WriteResponseMessage()
        {
            var pendingProcessing = await _repository.GetPendingAsync(default);
            return JsonConvert.SerializeObject(pendingProcessing);
        }

        internal async Task<string> HandleProcessedBatchMessage(byte[] body)
        {
            var receivedMessage = Encoding.UTF8.GetString(body);
            var processedBatch = JsonConvert.DeserializeObject<List<Billing>>(receivedMessage);
            await _repository.UpdateProcessedBatchAsync(processedBatch);
            return receivedMessage;
        }
    }
}
