using System;
using System.ComponentModel.DataAnnotations;

namespace Issuance.Api.Application.Models
{
    public class BillingResponse
    {
        [Required] public Guid Id { get; set; }
        [Required] public string Cpf { get; set; }
        [Required] public string DueDate { get; set; }
        [Required] public double Amount { get; set; }
    }
}
