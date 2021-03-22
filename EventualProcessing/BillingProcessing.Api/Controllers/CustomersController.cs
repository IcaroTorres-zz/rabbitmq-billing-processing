using BillingProcessing.Api.Application.Requests;
using BillingProcessing.Api.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Abstractions;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator mediator;

        public CustomersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut("{cpf}")]
        public async Task<IActionResult> ToggleCustomerBillingsAsync(ulong cpf, [FromBody] ToggleCustomerBillingsRequest request)
        {
            var usecaseRequest = request.Active
                ? new Customer { Cpf = cpf, State = request.State }
                : new DisableCustomerRequest
                {
                    CpfString = cpf.ToString("00000000000"),
                    CpfLong = cpf
                } as IRequest<IResult>;

            return await mediator.Send(usecaseRequest);
        }
    }
}