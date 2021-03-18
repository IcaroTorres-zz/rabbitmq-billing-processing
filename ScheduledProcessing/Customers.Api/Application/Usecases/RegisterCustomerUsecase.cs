using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using Customers.Api.Application.Responses;
using Customers.Api.Domain.Services;
using Library.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Usecases
{
    public class RegisterCustomerUsecase : IRequestHandler<RegisterCustomerRequest, IResult>
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IModelFactory _factory;
        private readonly ICustomerRepository _repository;

        public RegisterCustomerUsecase(
            IUnitofwork uow,
            IModelFactory factory,
            ICustomerRepository repository)
        {
            _unitofwork = uow;
            _factory = factory;
            _repository = repository;
        }

        public async Task<IResult> Handle(RegisterCustomerRequest request, CancellationToken cancellationToken)
        {
            _unitofwork.BeginTransaction();
            var customer = _factory.CreateCustomer(request.Cpf, request.Name, request.State);
            await _repository.InsertAsync(customer, cancellationToken);
            var transactionResult = await _unitofwork.CommitAsync(cancellationToken);
            return transactionResult.IsSuccess()
                ? new CreatedWithLocationResult<CustomerResponse>(new CustomerResponse(customer), request)
                : transactionResult;
        }
    }
}
