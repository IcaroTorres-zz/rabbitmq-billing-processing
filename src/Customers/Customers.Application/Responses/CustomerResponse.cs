using Customers.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Customers.Application.Responses
{
    public class CustomerResponse
    {
        public CustomerResponse(Customer customer)
        {
            Cpf = customer.Cpf.ToString().PadLeft(11, '0');
            Name = customer.Name;
            State = customer.State;
        }
        [Required] public string Cpf { get; internal set; }
        [Required] public string Name { get; internal set; }
        [Required] public string State { get; internal set; }
    }
}
