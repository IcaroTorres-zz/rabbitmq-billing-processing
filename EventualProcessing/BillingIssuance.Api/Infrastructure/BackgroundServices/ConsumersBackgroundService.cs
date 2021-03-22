using BillingIssuance.Api.Application.Usecases;
using BillingIssuance.Api.Domain.Models;
using Microsoft.Extensions.Hosting;
using PrivatePackage.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Infrastructure.BackgroundServices
{
    public class ConsumersBackgroundService : BackgroundService
    {
        private readonly IMessageConsumer<Customer> customerRegisteredConsumer;

        public ConsumersBackgroundService(IMessageConsumer<Customer> customerRegisteredConsumer)
        {
            this.customerRegisteredConsumer = customerRegisteredConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await customerRegisteredConsumer.ConsumeWithUsecase(nameof(EnabledCustomerUsecase));
        }
    }
}
