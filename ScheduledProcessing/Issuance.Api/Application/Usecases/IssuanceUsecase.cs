using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Models;
using Issuance.Api.Domain.Services;
using Library.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Usecases
{
    public class IssuanceUsecase : IRequestHandler<BillingRequest, IResult>
    {
        private readonly IModelFactory factory;
        private readonly IBillingRepository repository;

        public IssuanceUsecase(IModelFactory factory, IBillingRepository repository)
        {
            this.factory = factory;
            this.repository = repository;
        }

        public async Task<IResult> Handle(BillingRequest request, CancellationToken cancellationToken)
        {
            var billing = factory.CreateBilling(request.Cpf, request.Amount, request.DueDate);
            await repository.InsertAsync(billing, cancellationToken);
            var response = new BillingResponse(billing);
            return new SuccessResult(response, StatusCodes.Status201Created);
        }
    }
}
