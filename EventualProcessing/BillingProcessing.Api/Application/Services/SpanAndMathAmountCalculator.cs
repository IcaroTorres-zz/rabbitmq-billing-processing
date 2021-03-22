using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using System;
using System.Text;

namespace BillingProcessing.Api.Application.Services
{
    public class SpanAndMathAmountCalculator : IAmountCalculator
    {
        public decimal Calculate(Customer customer)
        {
            var span = new StringBuilder()
                .Append(customer.Cpf)
                .ToString()
                .AsSpan();

            var value = (span[^1] - '0');
            value += 10 * (span[^2] - '0');
            if (span.Length >= 10) value += 100 * (span[^10] - '0');
            if (span.Length >= 11) value += 1000 * (span[^11] - '0');

            return value;
        }
    }
}
