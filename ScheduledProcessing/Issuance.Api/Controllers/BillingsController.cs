﻿using Issuance.Api.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Library.Caching;
using System.Threading.Tasks;
using Library.Results;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Issuance.Api.Controllers
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

        /// <summary>
        /// Exposes a method that takes as parameters the Due Date, CPF and the amount of the billing.
        /// The Due Date must be future date in given format [dd-MM-yyyy], the CPF must be valid and
        /// Amount be greather tham zero.
        /// </summary>
        /// <param name="request"></param>
        /// <returns code="201">Created result with the ordered billing.</returns>
        [ProducesResponseType(typeof(Output<BillingResponse>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpPost]
        public async Task<IActionResult> OrderBillingAsync([FromBody] BillingRequest request)
        {
            return await mediator.Send(request);
        }

        /// <summary>
        /// A method that receives a CPF or a reference month as a parameter and returns the billingss registered
        /// according to the filter. At least one of the filters is mandatory (Cpf and/or Month as [MM-yyyy]).
        /// </summary>
        /// <param name="request"></param>
        /// <returns code="200">OK result with billings matcing the filters.</returns>
        [ProducesResponseType(typeof(Output<List<BillingResponse>>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Output<object>))]
        [HttpGet, Cache(15)]
        public async Task<IActionResult> GetBillingsAsync([FromQuery] GetBillingsRequest request)
        {
            return await mediator.Send(request);
        }
    }
}