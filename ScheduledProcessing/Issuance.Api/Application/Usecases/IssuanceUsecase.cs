using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Models;
using Issuance.Api.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Library.Abstractions;
using Library.Results;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Usecases
{
    public class IssuanceUsecase : IRequestHandler<BillingRequest, IResult>
    {
        private readonly IModelFactory factory;
        private readonly IBillingRepository repository;
        private readonly IResponseConverter converter;

        public IssuanceUsecase(IModelFactory factory, IBillingRepository repository, IResponseConverter converter)
        {
            this.factory = factory;
            this.repository = repository;
            this.converter = converter;
        }

        public async Task<IResult> Handle(BillingRequest request, CancellationToken cancellationToken)
        {
            var billing = factory.CreateBilling(request.Cpf, request.Amount, request.DueDate);
            await repository.InsertAsync(billing, cancellationToken);
            var response = converter.ToResponse(billing);
            return new SuccessResult(response, StatusCodes.Status201Created);
        }
    }
}
