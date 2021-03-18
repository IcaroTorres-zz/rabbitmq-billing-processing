using Issuance.Api.Domain.Models;
using System;

namespace Issuance.Api.Domain.Services
{
    /// <summary>
    /// Implementation for creating Domain Models
    /// </summary>
    public interface IModelFactory
    {
        Billing CreateBilling(ReadOnlySpan<char> cpfString, double amount, ReadOnlySpan<char> dueDate);
    }
}
