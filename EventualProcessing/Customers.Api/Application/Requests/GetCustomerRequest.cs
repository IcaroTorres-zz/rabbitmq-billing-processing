using MediatR;
using PrivatePackage.Abstractions;

namespace Customers.Api.Application.Requests
{
    public class GetCustomerRequest : IRequest<IResult>
    {
        public string Cpf { get; set; }
    }
}
