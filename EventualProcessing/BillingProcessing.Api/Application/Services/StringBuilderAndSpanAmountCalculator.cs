using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using PrivatePackage.Optmizations;
using System;
using System.Text;

namespace BillingProcessing.Api.Application.Services
{
    public class StringBuilderAndSpanAmountCalculator : IAmountCalculator
    {
        public decimal Calculate(Customer customer)
        {
            var sb = new StringBuilder();
            return sb.Append(customer.Cpf)
                .Remove(sb.Length - 9, 7)
                .ToString()
                .AsSpan()
                .ParseUshort();
        }
    }
}
