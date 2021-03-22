﻿using Customers.Api.Application.Requests;
using Customers.Api.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrivatePackage.Caching;
using PrivatePackage.Results;
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

        /// <summary>
        /// The registration method receive as parameters: Name (string), State (string), CPF (string).
        /// Validate the CPF and duplicate CPFs or empty fields are not accepted.
        /// </summary>
        /// <param name="request"></param>
        /// <returns code="201">Created result with link to resource on location in header.</returns>
        [ProducesResponseType(typeof(Output<Customer>), StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpPost]
        public async Task<IActionResult> RegisterCostumer([FromBody] RegisterCustomerRequest request)
        {
            return await mediator.Send(request.SetupForCreation(Url, getCustomerRoute, x => new { cpf = x.Cpf }));
        }

        /// <summary>
        /// The Query method receive and validates a CPF (string) as parameters and perform the query.
        /// </summary>
        /// <param name="request"></param>
        /// <returns code="200">Ok result with a customer in response if some.</returns>
        [ProducesResponseType(typeof(Output<Customer>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpGet("{cpf}", Name = getCustomerRoute), Cache(15)]
        public async Task<IActionResult> GetCostumer([FromRoute] GetCustomerRequest request)
        {
            return await mediator.Send(request);
        }
    }
}