using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Models;
using Issuance.Api.Domain.Services;
using Library.Messaging;
using Library.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Usecases
{
    public class IssuanceUsecase : IRequestHandler<BillingRequest, IResult>
    {
        private readonly IModelFactory _factory;
        private readonly IBillingRepository _repository;
        private readonly IMessagePublisher _publisher;

        public IssuanceUsecase(IModelFactory factory, IBillingRepository repository, IMessagePublisher publisher)
        {
            _factory = factory;
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<IResult> Handle(BillingRequest request, CancellationToken cancellationToken)
        {
            var billing = _factory.CreateBilling(request.Cpf, request.Amount, request.DueDate);
            await _repository.InsertAsync(billing, cancellationToken);
            /*
             * Sem confirmação a título de propótipo. Use BasicConfirmedMessage se deseja confirmações de publicação
             */
            await _publisher.Publish(new BasicMessage(billing, nameof(IssuanceUsecase)));
            var response = new BillingResponse(billing);
            return new SuccessResult(response, StatusCodes.Status201Created);
        }
    }
}
