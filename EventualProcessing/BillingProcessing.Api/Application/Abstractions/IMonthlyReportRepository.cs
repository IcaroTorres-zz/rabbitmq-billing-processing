using BillingProcessing.Api.Application.Responses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Abstractions
{
    public interface IMonthlyReportRepository
    {
        Task<List<MonthlyReportResponse>> MapReduceMonthlyBillingByStateAsync(CancellationToken token);
    }
}
