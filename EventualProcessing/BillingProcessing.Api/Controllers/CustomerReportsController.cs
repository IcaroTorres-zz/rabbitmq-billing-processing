using BillingProcessing.Api.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Caching;
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

        [HttpGet("{cpf}"), Cache(15)]
        public async Task<IActionResult> GetCustomerBillingsAsync(string cpf, [FromQuery] CustomerReportRequest request)
        {
            request.Cpf = cpf;
            return await mediator.Send(request);
        }
    }
}
