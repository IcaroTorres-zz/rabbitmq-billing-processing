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
    [Route("api/monthly-reports")]
    [ApiController]
    public class MonthlyReportsController : ControllerBase
    {
        private readonly IMediator mediator;

        public MonthlyReportsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Gets a Monthly billing Report for processed billings with total and grouped by region States with Its accumulated totals.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(Output<MonthlyReportResponse>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpGet, Cache(60)]
        public async Task<IActionResult> GetMonthlyReportAsync()
        {
            return await mediator.Send(new MonthlyReportRequest());
        }
    }
}
