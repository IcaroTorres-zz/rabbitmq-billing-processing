using Customers.Application.Abstractions;
using Customers.Domain.Models;
using Library.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;

namespace Customers.Application.Workers
{
    public class ScheduledCustomerAcceptProcessWorker : RpcServer<ScheduledCustomerAcceptProcessWorker>
    {
        private readonly ICustomerRepositoryFactory _repositoryFactory;

        public ScheduledCustomerAcceptProcessWorker(IConnectionFactory factory, ICustomerRepositoryFactory repositoryFactory, ILogger<ScheduledCustomerAcceptProcessWorker> logger)
            : base(nameof(Customer), factory, logger)
        {
            _repositoryFactory = repositoryFactory;
        }

        public override Task<string> HandleReceivedMessage(BasicDeliverEventArgs ea) => Task.FromResult(string.Empty);

        public override async Task<string> WriteResponseMessage()
        {
            var repository = _repositoryFactory.CreateRepository();
            var customers = await repository.GetAllAsync();
            return JsonConvert.SerializeObject(customers);
        }
    }
}
