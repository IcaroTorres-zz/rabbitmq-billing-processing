using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Models;
using Library.Abstractions;
using Library.Optimizations;
using Library.Results;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Usecases
{
    public class GetBillingsUsecase : IRequestHandler<GetBillingsRequest, IResult>
    {
        private readonly IBillingRepository repository;

        public GetBillingsUsecase(IBillingRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IResult> Handle(GetBillingsRequest request, CancellationToken cancellationToken)
        {
            var cpf = ExtractCpf(request);
            var (month, year) = ExtractMonthYear(request);
            var billings = await repository.GetManyAsync(cpf, month, year, cancellationToken);
            var responses = billings.ConvertAll(x => new BillingResponse(x));
            return new SuccessResult(responses);
        }

        private ulong ExtractCpf(GetBillingsRequest request)
        {
            ReadOnlySpan<char> cpf = request.Cpf ?? "0";
            return cpf.ParseUlong();
        }

        private (byte, ushort) ExtractMonthYear(GetBillingsRequest request)
        {
            ReadOnlySpan<char> month = request.Month ?? "00-0000";
            var monthPart = month.Slice(0, 2).ParseByte();
            var yearPart = month.Slice(3, 4).ParseUshort();
            return (monthPart, yearPart);
        }
    }
}
