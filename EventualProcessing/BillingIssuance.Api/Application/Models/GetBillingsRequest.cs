using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Abstractions;

namespace BillingIssuance.Api.Application.Models
{
    public class GetBillingsRequest : IRequest<IResult>
    {
        [FromQuery] public string? Cpf { get; set; }
        [FromQuery] public string? Month { get; set; }
    }
}
