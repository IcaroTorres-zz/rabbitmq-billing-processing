using Processing.Worker.Domain.Models;
using System;
using System.Text;

namespace Processing.Worker.Domain.Services
{
    public class SpanAndMathAmountProcessor : IAmountProcessor
    {
        public Billing Process(Customer customer, Billing billing)
        {
            var span = new StringBuilder()
                .Append(customer.Cpf)
                .ToString()
                .AsSpan();

            var value = span[^1] - '0';
            value += 10 * (span[^2] - '0');
            if (span.Length >= 10) value += 100 * (span[^10] - '0');
            if (span.Length >= 11) value += 1000 * (span[^11] - '0');

            billing.Amount = value;
            billing.ProcessedAt = DateTime.UtcNow;
            return billing;
        }
    }
}
