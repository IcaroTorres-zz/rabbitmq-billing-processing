using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Usecases
{
    public class HandleIssuanceUsecase : IRequestHandler<Billing, IResult>
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IBillingsRepository billingRepository;
        private readonly IAmountCalculator calculator;

        public HandleIssuanceUsecase(
            ICustomerRepository customerRepository,
            IBillingsRepository billingRepository,
            IAmountCalculator calculator)
        {
            this.customerRepository = customerRepository;
            this.billingRepository = billingRepository;
            this.calculator = calculator;
        }

        public async Task<IResult> Handle(Billing billing, CancellationToken cancellationToken)
        {
            var customer = await customerRepository.GetAsync(billing.Cpf, cancellationToken);
            customer.AcceptProcessing(billing, calculator);
            billing = await billingRepository.InsertAsync(billing, cancellationToken);
            return new SuccessResult(billing);
        }
    }
}
