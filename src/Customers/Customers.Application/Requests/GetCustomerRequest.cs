using Library.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Customers.Application.Requests
{
    public class GetCustomerRequest : IRequest<IResult>
    {
        [Required] public string Cpf { get; set; }
    }
}
