using System;

namespace BillingIssuance.Api.Application.Models
{
    public class BillingResponse
    {
        public Guid Id { get; set; }
        public string Cpf { get; set; }
        public string DueDate { get; set; }
        public double Amount { get; set; }
    }
}
