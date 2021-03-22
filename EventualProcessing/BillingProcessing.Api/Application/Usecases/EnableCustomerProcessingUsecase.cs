using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Usecases
{
    public class EnableCustomerProcessingUsecase : IRequestHandler<Customer, IResult>
    {
        private readonly IBillingsRepository billingRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IAmountCalculator calculator;

        public EnableCustomerProcessingUsecase(
            IBillingsRepository billingRepository,
            ICustomerRepository customerRepository,
            IAmountCalculator calculator)
        {
            this.billingRepository = billingRepository;
            this.customerRepository = customerRepository;
            this.calculator = calculator;
        }

        public async Task<IResult> Handle(Customer request, CancellationToken cancellationToken)
        {
            var pendingBillingsTask = billingRepository.GetCustomerPendingBillingsAsync(request.Cpf, cancellationToken);

            request.EnableProcessing();
            await customerRepository.InsertOrUpdateAsync(request, cancellationToken);

            var pendingBillings = await pendingBillingsTask;

            await Task.Run(() =>
            {
                Parallel.ForEach(pendingBillings, billing => request.AcceptProcessing(billing, calculator));
            }, cancellationToken);

            await billingRepository.UpdateManyProcessedAsync(pendingBillings, cancellationToken);

            return new SuccessResult(request);
        }
    }
}
