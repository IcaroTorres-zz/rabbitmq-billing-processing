using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using Customers.Api.Application.Responses;
using Customers.Api.Domain.Models;
using Customers.Api.Domain.Services;
using Issuance.Api.Application.Abstractions;
using Library.Abstractions;
using Library.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Usecases
{
    public class RegisterCustomerUsecase : IRequestHandler<RegisterCustomerRequest, IResult>
    {
        private readonly IUnitofwork uow;
        private readonly IModelFactory factory;
        private readonly ICustomerRepository repository;
        private readonly IResponseConverter converter;

        public RegisterCustomerUsecase(
            IUnitofwork uow,
            IModelFactory factory,
            ICustomerRepository repository,
            IResponseConverter converter)
        {
            this.uow = uow;
            this.factory = factory;
            this.repository = repository;
            this.converter = converter;
        }

        public async Task<IResult> Handle(RegisterCustomerRequest request, CancellationToken cancellationToken)
        {
            uow.BeginTransaction();
            var customer = factory.CreateCustomer(request.Cpf, request.Name, request.State);
            await repository.InsertAsync(customer, cancellationToken);
            var transactionResult = await uow.CommitAsync(cancellationToken);
            return transactionResult.IsSuccess()
                ? new CreatedWithLocationResult<CustomerResponse>(converter.ToResponse(customer), request)
                : transactionResult;
        }
    }
}
