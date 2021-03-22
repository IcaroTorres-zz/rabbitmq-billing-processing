using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using PrivatePackage.Optmizations;
using System;

namespace BillingProcessing.Api.Application.Services
{
    public class ToStringAmountCalculator : IAmountCalculator
    {
        public decimal Calculate(Customer customer)
        {
            var str = customer.Cpf.ToString("00000000000");
            ReadOnlySpan<char> span = new char[4] { str[0], str[1], str[9], str[10] }.AsSpan();
            return span.ParseUshort();
        }
    }
}
