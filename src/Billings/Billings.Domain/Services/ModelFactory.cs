using Billings.Domain.Models;
using Library.Optimizations;
using System;

namespace Billings.Domain.Services
{
    /// <inheritdoc cref="IModelFactory"/>
    public class ModelFactory : IModelFactory
    {
        public Billing CreateBilling(string cpfString, double amount, string dueDate)
        {
            return new Billing
            {
                Id = Guid.NewGuid(),
                Cpf = cpfString.AsSpan().ParseUlong(),
                Amount = amount,
                DueDate = new Date(dueDate)
            };
        }
    }
}
