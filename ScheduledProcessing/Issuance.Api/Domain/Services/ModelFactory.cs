using Issuance.Api.Domain.Models;
using Library.Optimizations;
using System;

namespace Issuance.Api.Domain.Services
{
    /// <inheritdoc cref="IModelFactory"/>
    public class ModelFactory : IModelFactory
    {
        public Billing CreateBilling(ReadOnlySpan<char> cpfString, double amount, ReadOnlySpan<char> dueDate)
        {
            return new Billing
            {
                Id = Guid.NewGuid(),
                Cpf = cpfString.ParseUlong(),
                Amount = amount,
                DueDate = new Date
                {
                    Day = dueDate.Slice(0, 2).ParseByte(),
                    Month = dueDate.Slice(3, 2).ParseByte(),
                    Year = dueDate.Slice(6, 4).ParseUshort()
                }
            };
        }
    }
}
