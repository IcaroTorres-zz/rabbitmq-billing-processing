using Customers.Api.Application.Responses;
using Library.Requests;
using System.ComponentModel.DataAnnotations;

namespace Customers.Api.Application.Requests
{
    public class RegisterCustomerRequest : CreationRequestBase<CustomerResponse>
    {
        [Required] public string Cpf { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string State { get; set; }
    }
}
