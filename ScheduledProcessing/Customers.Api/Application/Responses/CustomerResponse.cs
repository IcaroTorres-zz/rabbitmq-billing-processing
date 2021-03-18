using System.ComponentModel.DataAnnotations;

namespace Customers.Api.Application.Responses
{
    public class CustomerResponse
    {
        [Required] public string Cpf { get; internal set; }
        [Required] public string Name { get; internal set; }
        [Required] public string State { get; internal set; }
    }
}
