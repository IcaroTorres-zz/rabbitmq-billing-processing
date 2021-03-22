using BillingProcessing.Api.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Caching;
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

        [HttpGet, Cache(1)]
        public async Task<IActionResult> GetMonthlyReportAsync()
        {
            return await mediator.Send(new MonthlyReportRequest());
        }
    }
}
