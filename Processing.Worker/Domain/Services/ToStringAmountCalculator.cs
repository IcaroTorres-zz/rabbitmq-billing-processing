using Library.Optimizations;
using Processing.Worker.Domain.Models;
using System;

namespace Processing.Worker.Domain.Services
{
    public class ToStringAmountProcessor : IAmountProcessor
    {
        public Billing Process(Customer customer, Billing billing)
        {
            var str = customer.Cpf.ToString("00000000000");
            ReadOnlySpan<char> span = new char[4] { str[0], str[1], str[9], str[10] }.AsSpan();
            billing.Amount = span.ParseUshort();
            billing.ProcessedAt = DateTime.UtcNow;
            return billing;
        }
    }
}
