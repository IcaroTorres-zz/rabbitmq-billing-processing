using Customers.Api.Domain.Models;
using PrivatePackage.Optmizations;
using System;

namespace Customers.Api.Domain.Services
{
    public class ModelFactory : IModelFactory
    {
        public Customer CreateCustomer(ReadOnlySpan<char> cpf, string name, string state)
        {
            return new Customer
            {
                Cpf = cpf.ParseUlong(),
                Name = name.ToUpperInvariant(),
                State = state.ToUpperInvariant()
            };
        }
    }
}
