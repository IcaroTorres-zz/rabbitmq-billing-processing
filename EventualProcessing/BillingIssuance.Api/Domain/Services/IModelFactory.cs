using BillingIssuance.Api.Domain.Models;
using System;

namespace BillingIssuance.Api.Domain.Services
{
    public interface IModelFactory
    {
        Billing CreateBilling(ReadOnlySpan<char> cpfString, double amount, ReadOnlySpan<char> dueDate);
    }
}
