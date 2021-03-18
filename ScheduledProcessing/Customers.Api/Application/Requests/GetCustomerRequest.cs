using Library.Abstractions;
using MediatR;

namespace Customers.Api.Application.Requests
{
    public class GetCustomerRequest : IRequest<IResult>
    {
        public string Cpf { get; set; }
    }
}
