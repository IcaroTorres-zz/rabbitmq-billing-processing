using BillingProcessing.Api.Application.Usecases;
using BillingProcessing.Api.Domain.Models;
using Microsoft.Extensions.Hosting;
using PrivatePackage.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Workers
{
    public class ConsumersBackgroundService : BackgroundService
    {
        private readonly IMessageConsumer<Customer> _customerRegisteredConsumer;
        private readonly IMessageConsumer<Billing> _billingIssuedConsumer;

        public ConsumersBackgroundService(IMessageConsumer<Customer> customerRegisteredConsumer, IMessageConsumer<Billing> billingIssuedConsumer)
        {
            this._customerRegisteredConsumer = customerRegisteredConsumer;
            this._billingIssuedConsumer = billingIssuedConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.WhenAll(
                _customerRegisteredConsumer.ConsumeWithUsecase(nameof(EnableCustomerProcessingUsecase)),
                _billingIssuedConsumer.ConsumeWithUsecase(nameof(HandleIssuanceUsecase)));
        }
    }
}
