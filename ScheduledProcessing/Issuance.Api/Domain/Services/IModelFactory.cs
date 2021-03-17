using Issuance.Api.Domain.Models;
using System;

namespace Issuance.Api.Domain.Services
{
    public interface IModelFactory
    {
        Billing CreateBilling(ReadOnlySpan<char> cpfString, double amount, ReadOnlySpan<char> dueDate);
    }
}
