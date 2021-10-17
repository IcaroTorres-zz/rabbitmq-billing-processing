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
    public class ScheduledBillingsToProcessWorker : RpcServer<List<Billing>>
    {
        private readonly IBillingRepository _repository;

        public ScheduledBillingsToProcessWorker(IConnectionFactory factory, IBillingRepository repository, ILogger logger) : base(nameof(Billing), factory, logger)
        {
            _repository = repository;
        }

        public override async Task<(List<Billing> receivedValue, string receivedMessage)> HandleReceivedMessage(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var receivedMessage = Encoding.UTF8.GetString(body);
            var processedBatch = JsonConvert.DeserializeObject<List<Billing>>(receivedMessage);
            await _repository.UpdateProcessedBatchAsync(processedBatch);
            return (processedBatch, receivedMessage);
        }

        public override async Task<string> WriteResponseMessage(List<Billing> receivedValue)
        {
            var pendingProcessing = await _repository.GetPendingAsync(default);
            return JsonConvert.SerializeObject(pendingProcessing);
        }
    }
}
