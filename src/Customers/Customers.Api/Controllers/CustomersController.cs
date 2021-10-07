using Customers.Application.Requests;
using Customers.Application.Responses;
using Library.Caching;
using Library.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Customers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private const string _getCustomerRoute = "get-customer";

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// The registration method receive as parameters: Name (string), State (string), CPF (string).
        /// Validate the CPF and duplicate CPFs or empty fields are not accepted.
        /// </summary>
        /// <param name="request"></param>
        /// <returns code="201">Created result with link to resource on location in header.</returns>
        [ProducesResponseType(typeof(Output<CustomerResponse>), StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpPost]
        public async Task<IActionResult> RegisterCostumer([FromBody] RegisterCustomerRequest request)
        {
            return await _mediator.Send(request.SetupForCreation(Url, _getCustomerRoute, x => new { cpf = x.Cpf }));
        }

        /// <summary>
        /// The Query method receive and validates a CPF (string) as parameters and perform the query.
        /// </summary>
        /// <param name="request"></param>
        /// <returns code="200">Ok result with a customer in response if some.</returns>
        [ProducesResponseType(typeof(Output<CustomerResponse>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpGet("{cpf}", Name = _getCustomerRoute), Cache(15)]
        public async Task<IActionResult> GetCostumer([FromRoute] GetCustomerRequest request)
        {
            return await _mediator.Send(request);
        }
    }
}
