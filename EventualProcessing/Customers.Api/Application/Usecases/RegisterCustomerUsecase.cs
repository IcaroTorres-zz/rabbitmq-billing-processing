using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using Customers.Api.Domain.Models;
using Customers.Api.Domain.Services;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Messaging;
using PrivatePackage.Results;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Usecases
{
    public class RegisterCustomerUsecase : IRequestHandler<RegisterCustomerRequest, IResult>
    {
        private readonly IUnitofwork uow;
        private readonly IModelFactory factory;
        private readonly ICustomerRepository repository;
        private readonly IMessagePublisher publisher;

        public RegisterCustomerUsecase(
            IUnitofwork uow,
            IModelFactory factory,
            ICustomerRepository repository,
            IMessagePublisher publisher)
        {
            this.uow = uow;
            this.factory = factory;
            this.repository = repository;
            this.publisher = publisher;
        }

        public async Task<IResult> Handle(RegisterCustomerRequest request, CancellationToken cancellationToken)
        {
            uow.BeginTransaction();
            var customer = factory.CreateCustomer(request.Cpf, request.Name, request.State);
            await repository.InsertAsync(customer, cancellationToken);
            var transactionResult = await uow.CommitAsync(cancellationToken);
            if (transactionResult.IsSuccess())
            {
                /*
                 * Sem confirmação a título de propótipo. Use BasicConfirmedMessage se deseja confirmações de publicação
                 */
                var message = new BasicMessage(customer, nameof(RegisterCustomerUsecase));
                var publishTask = publisher.Publish(message);
                return new CreatedWithLocationResult<Customer>(customer, request);
            }
            return transactionResult;
        }
    }
}
