using BillingProcessing.Api.Application.Requests;
using BillingProcessing.Api.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Abstractions;
using PrivatePackage.Results;
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

        /// <summary>
        /// Validates the Cpf and checks if there is a Customer to toggle Its activity status returning It.
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="request"></param>
        /// <returns code="200">Ok result with a toggled customer in response if some.</returns>
        [ProducesResponseType(typeof(Output<Customer>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Output<object>))]
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