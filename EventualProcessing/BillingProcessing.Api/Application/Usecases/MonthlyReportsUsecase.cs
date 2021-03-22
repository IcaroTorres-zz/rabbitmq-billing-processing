using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Requests;
using MediatR;
using PrivatePackage.Abstractions;
using PrivatePackage.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Usecases
{
    public class MonthlyReportsUsecase : IRequestHandler<MonthlyReportRequest, IResult>
    {
        private readonly IMonthlyReportRepository repository;

        public MonthlyReportsUsecase(IMonthlyReportRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IResult> Handle(MonthlyReportRequest request, CancellationToken cancellationToken)
        {
            var monthlyReport = await repository.MapReduceMonthlyBillingByStateAsync(cancellationToken);
            return new SuccessResult(monthlyReport);
        }
    }
}
