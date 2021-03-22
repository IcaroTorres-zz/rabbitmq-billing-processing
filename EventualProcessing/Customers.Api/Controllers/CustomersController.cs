using Customers.Api.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Caching;
using System.Threading.Tasks;

namespace Customers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator mediator;
        private const string getCustomerRoute = "get-customer";

        public CustomersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCostumer([FromBody] RegisterCustomerRequest request)
        {
            return await mediator.Send(request.SetupForCreation(Url, getCustomerRoute, x => new { cpf = x.Cpf }));
        }

        [HttpGet("{cpf}", Name = getCustomerRoute), Cache(15)]
        public async Task<IActionResult> GetCostumer([FromRoute] GetCustomerRequest request)
        {
            return await mediator.Send(request);
        }
    }
}
