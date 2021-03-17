using Library.Optimizations;
using Processing.Worker.Domain.Models;
using System;
using System.Text;

namespace Processing.Worker.Domain.Services
{
    public class StringBuilderAndSpanAmountProcessor : IAmountProcessor
    {
        public Billing Process(Customer customer, Billing billing)
        {
            var sb = new StringBuilder();
            billing.Amount = sb.Append(customer.Cpf)
                .Remove(sb.Length - 9, 7)
                .ToString()
                .AsSpan()
                .ParseUshort();
            billing.ProcessedAt = DateTime.UtcNow;
            return billing;
        }
    }
}
