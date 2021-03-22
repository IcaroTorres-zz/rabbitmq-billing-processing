namespace BillingProcessing.Api.Application.Responses
{
    public class BillingResponse
    {
        public string DueDate { get; set; }
        public string ProcessedAt { get; set; }
        public decimal Amount { get; set; }
    }
}
