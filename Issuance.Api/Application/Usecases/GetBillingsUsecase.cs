using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Models;
using Library.Results;
using Library.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Usecases
{
    public class GetBillingsUsecase : IRequestHandler<GetBillingsRequest, IResult>
    {
        private readonly IBillingRepository _repository;

        public GetBillingsUsecase(IBillingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IResult> Handle(GetBillingsRequest request, CancellationToken cancellationToken)
        {
            Cpf.TryParse(request.Cpf, out var cpf);
            Date.TryParseMonth(request.Month ?? "00-0000", out var month, out var year);
            var billings = await _repository.GetManyAsync(cpf, month, year, cancellationToken);
            var responses = billings.ConvertAll(x => new BillingResponse(x));
            return new SuccessResult(responses);
        }
    }
}
