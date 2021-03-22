using BillingProcessing.Api.Application.Requests;
using BillingProcessing.Api.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Caching;
using PrivatePackage.Results;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Controllers
{
    [Route("api/customer-reports")]
    [ApiController]
    public class CustomerReportsController : ControllerBase
    {
        private readonly IMediator mediator;

        public CustomerReportsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Validates the Cpf and checks if there is a Customer enable for processing with It and query
        /// all billings issued for It in between given range.
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="request"></param>
        /// <returns code="200">Ok result with a customer report in response if some.</returns>
        [ProducesResponseType(typeof(Output<CustomerReportResponse>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpGet("{cpf}"), Cache(15)]
        public async Task<IActionResult> GetCustomerBillingsAsync(string cpf, [FromQuery] CustomerReportRequest request)
        {
            request.Cpf = cpf;
            return await mediator.Send(request);
        }
    }
}
