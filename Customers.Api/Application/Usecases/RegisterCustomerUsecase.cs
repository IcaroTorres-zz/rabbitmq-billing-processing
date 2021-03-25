using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using Customers.Api.Application.Responses;
using Customers.Api.Domain.Services;
using Library.Messaging;
using Library.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Usecases
{
    public class RegisterCustomerUsecase : IRequestHandler<RegisterCustomerRequest, IResult>
    {
        private readonly IUnitofwork _uow;
        private readonly IModelFactory _factory;
        private readonly ICustomerRepository _repository;
        private readonly IMessagePublisher _publisher;

        public RegisterCustomerUsecase(
            IUnitofwork uow,
            IModelFactory factory,
            ICustomerRepository repository,
            IMessagePublisher publisher)
        {
            _uow = uow;
            _factory = factory;
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<IResult> Handle(RegisterCustomerRequest request, CancellationToken cancellationToken)
        {
            _uow.BeginTransaction();
            var customer = _factory.CreateCustomer(request.Cpf, request.Name, request.State);
            await _repository.InsertAsync(customer, cancellationToken);
            var transactionResult = await _uow.CommitAsync(cancellationToken);
            if (transactionResult.IsSuccess())
            {
                /*
                 * Sem confirmação a título de propótipo. Use BasicConfirmedMessage se deseja confirmações de publicação
                 */
                var message = new BasicMessage(customer, nameof(RegisterCustomerUsecase));
                await _publisher.Publish(message);
                var response = new CustomerResponse(customer);
                return new CreatedWithLocationResult<CustomerResponse>(response, request);
            }
            return transactionResult;
        }
    }
}
