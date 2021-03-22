using Customers.Api.Domain.Models;
using PrivatePackage.Abstractions;

namespace Customers.Api.Application.Requests
{
    public class RegisterCustomerRequest : CreationRequestBase<Customer>
    {
        public string Cpf { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }
}
