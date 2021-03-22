using System.Collections.Generic;

namespace BillingProcessing.Api.Application.Responses
{
    public class MonthlyReportResponse
    {
        public string Month { get; set; }
        public decimal Total { get; set; }
        public List<StateReportResponse> States { get; set; }
    }
}
