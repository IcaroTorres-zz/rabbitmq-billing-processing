using System.Collections.Generic;

namespace BillingProcessing.Api.Application.Responses
{
    public class CustomerReportResponse
    {
        public string Cpf { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public string StartDate { get; internal set; }
        public string EndDate { get; internal set; }
        public List<BillingResponse> Billings { get; set; }
    }
}
