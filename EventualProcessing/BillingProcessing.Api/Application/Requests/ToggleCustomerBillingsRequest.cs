namespace BillingProcessing.Api.Application.Requests
{
    public class ToggleCustomerBillingsRequest
    {
        public string State { get; set; }
        public bool Active { get; set; }
    }
}