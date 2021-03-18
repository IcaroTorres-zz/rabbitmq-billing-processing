using Library.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Issuance.Api.Application.Models
{
    public class GetBillingsRequest : IRequest<IResult>
    {
        [FromQuery] public string? Cpf { get; set; }
        [FromQuery] public string? Month { get; set; }
    }
}
