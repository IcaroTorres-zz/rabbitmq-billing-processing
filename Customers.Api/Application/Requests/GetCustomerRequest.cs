using MediatR;
using Library.Abstractions;

namespace Customers.Api.Application.Requests
{
    public class GetCustomerRequest : IRequest<IResult>
    {
        public string Cpf { get; set; }
    }
}
