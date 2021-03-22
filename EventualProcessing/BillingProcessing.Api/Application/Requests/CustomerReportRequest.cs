using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Abstractions;

namespace BillingProcessing.Api.Application.Requests
{
    public class CustomerReportRequest : IRequest<IResult>
    {
        public string Cpf { get; set; }
        [FromQuery] public string Begin { get; set; }
        [FromQuery] public string End { get; set; }
    }
}
