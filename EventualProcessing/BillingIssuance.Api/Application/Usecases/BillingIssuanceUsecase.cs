using BillingIssuance.Api.Application.Abstractions;
using BillingIssuance.Api.Application.Models;
using BillingIssuance.Api.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using PrivatePackage.Abstractions;
using PrivatePackage.Messaging;
using PrivatePackage.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Application.Usecases
{
    public class BillingIssuanceUsecase : IRequestHandler<BillingRequest, IResult>
    {
        private readonly IModelFactory factory;
        private readonly IBillingRepository repository;
        private readonly IMessagePublisher publisher;
        private readonly IResponseConverter converter;

        public BillingIssuanceUsecase(IModelFactory factory, IBillingRepository repository, IMessagePublisher publisher, IResponseConverter converter)
        {
            this.factory = factory;
            this.repository = repository;
            this.publisher = publisher;
            this.converter = converter;
        }

        public async Task<IResult> Handle(BillingRequest request, CancellationToken cancellationToken)
        {
            var billing = factory.CreateBilling(request.Cpf, request.Amount, request.DueDate);
            await repository.InsertAsync(billing, cancellationToken);
            /*
             * Sem confirmação a título de propótipo. Use BasicConfirmedMessage se deseja confirmações de publicação
             */
            await publisher.Publish(new BasicMessage(billing, nameof(BillingIssuanceUsecase)));
            var response = converter.ToResponse(billing);
            return new SuccessResult(response, StatusCodes.Status201Created);
        }
    }
}
