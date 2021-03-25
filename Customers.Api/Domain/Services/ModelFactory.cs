using Customers.Api.Domain.Models;
using Library.Optimizations;
using System;

namespace Customers.Api.Domain.Services
{
    /// <inheritdoc cref="IModelFactory"/>
    public class ModelFactory : IModelFactory
    {
        public Customer CreateCustomer(string cpf, string name, string state)
        {
            return new Customer
            {
                Cpf = cpf.AsSpan().ParseUlong(),
                Name = name.ToUpperInvariant(),
                State = state.ToUpperInvariant()
            };
        }
    }
}
