using Customers.Api.Domain.Models;
using System;

namespace Customers.Api.Domain.Services
{
    public interface IModelFactory
    {
        Customer CreateCustomer(ReadOnlySpan<char> cpf, string name, string state);
    }
}
