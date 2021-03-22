using BillingIssuance.Api.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Caching;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingsController : ControllerBase
    {
        private readonly IMediator mediator;

        public BillingsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> OrderBillingAsync([FromBody] BillingRequest request)
        {
            return await mediator.Send(request);
        }

        [HttpGet, Cache(15)]
        public async Task<IActionResult> GetBillingsAsync([FromQuery] GetBillingsRequest request)
        {
            return await mediator.Send(request);
        }
    }
}
