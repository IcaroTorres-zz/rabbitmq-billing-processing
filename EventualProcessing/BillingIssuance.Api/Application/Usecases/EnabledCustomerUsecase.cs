using BillingIssuance.Api.Application.Abstractions;
using BillingIssuance.Api.Domain.Models;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Application.Usecases
{
    public class EnabledCustomerUsecase : IRequestHandler<Customer, IResult>
    {
        private readonly ICustomerRepository repository;

        public EnabledCustomerUsecase(ICustomerRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IResult> Handle(Customer customer, CancellationToken cancellationToken)
        {
            customer.EnableCharges();
            await repository.InsertOrUpdateAsync(customer, cancellationToken);
            return new SuccessResult(customer);
        }
    }
}
