using MediatR;
using Microsoft.AspNetCore.Mvc;
using Library.Abstractions;

namespace Issuance.Api.Application.Models
{
    public class GetBillingsRequest : IRequest<IResult>
    {
        [FromQuery] public string? Cpf { get; set; }
        [FromQuery] public string? Month { get; set; }
    }
}
