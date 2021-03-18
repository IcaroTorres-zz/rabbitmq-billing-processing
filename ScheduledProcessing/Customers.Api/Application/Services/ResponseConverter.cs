using Customers.Api.Application.Responses;
using Customers.Api.Domain.Models;
using Issuance.Api.Application.Abstractions;

namespace Issuance.Api.Application.Services
{
    public class ResponseConverter : IResponseConverter
    {
        public CustomerResponse ToResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Cpf = customer.Cpf.ToString().PadLeft(11, '0'),
                Name = customer.Name,
                State = customer.State
            };
        }
    }
}
