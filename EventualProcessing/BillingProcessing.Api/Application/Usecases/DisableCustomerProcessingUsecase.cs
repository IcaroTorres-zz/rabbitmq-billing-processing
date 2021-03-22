using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Requests;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Usecases
{
    public class DisableCustomerProcessingUsecase : IRequestHandler<DisableCustomerRequest, IResult>
    {
        private readonly ICustomerRepository repository;

        public DisableCustomerProcessingUsecase(ICustomerRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IResult> Handle(DisableCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = await repository.GetAsync(request.CpfLong, cancellationToken);
            customer.DisableProcessing();
            await repository.InsertOrUpdateAsync(customer, cancellationToken);

            return new SuccessResult(customer);
        }
    }
}
