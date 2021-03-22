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

        public async Task<IResult> Handle(Billing request, CancellationToken cancellationToken)
        {
            var customer = await customerRepository.GetAsync(request.Cpf, cancellationToken);
            customer.AcceptProcessing(request, calculator);
            request = await billingRepository.InsertAsync(request, cancellationToken);
            return new SuccessResult(request);
        }
    }
}
