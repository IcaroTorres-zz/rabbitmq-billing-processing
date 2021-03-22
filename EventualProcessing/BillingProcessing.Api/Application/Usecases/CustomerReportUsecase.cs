using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Requests;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Optmizations;
using PrivatePackage.Results;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Usecases
{
    public class CustomerReportUsecase : IRequestHandler<CustomerReportRequest, IResult>
    {
        private readonly IBillingsRepository billingRepository;
        private readonly IResponseConverter converter;

        public CustomerReportUsecase(IBillingsRepository billingRepository, IResponseConverter converter)
        {
            this.billingRepository = billingRepository;
            this.converter = converter;
        }

        public async Task<IResult> Handle(CustomerReportRequest request, CancellationToken cancellationToken)
        {
            var cpf = request.Cpf.AsSpan().ParseUlong();
            var begin = ParseDateOrDefault(request.Begin, DateTime.MinValue);
            var end = ParseDateOrDefault(request.End, DateTime.MaxValue);

            var billings = await billingRepository.GetCustomerProcessedBillingsAsync(cpf, begin, end, cancellationToken);
            var response = converter.ToResponse(billings, begin, end);
            return new SuccessResult(response);
        }

        private DateTime ParseDateOrDefault(string date, DateTime defaultDate)
        {
            return DateTime.TryParse(date,
                CultureInfo.CreateSpecificCulture("pt-BR"),
                DateTimeStyles.AdjustToUniversal,
                out var parsedDate) ? parsedDate : defaultDate;
        }
    }
}
