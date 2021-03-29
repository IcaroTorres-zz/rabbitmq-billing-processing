using Billings.Application.Abstractions;
using Billings.Application.Models;
using Billings.Domain.Services;
using Library.Messaging;
using Library.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Billings.Application.Usecases
{
    public class BillingIssuanceUsecase : IRequestHandler<BillingRequest, IResult>
    {
        private readonly IModelFactory _factory;
        private readonly IBillingRepository _repository;
        private readonly IMessagePublisher _publisher;

        public BillingIssuanceUsecase(IModelFactory factory, IBillingRepository repository, IMessagePublisher publisher)
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
            await _publisher.Publish(new BasicMessage(billing, nameof(BillingIssuanceUsecase)));
            var response = new BillingResponse(billing);
            return new SuccessResult(response, StatusCodes.Status201Created);
        }
    }
}
