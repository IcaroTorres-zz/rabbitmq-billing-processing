using BillingIssuance.Api.Application.Models;
using BillingIssuance.Api.Domain.Models;
using System.Collections.Generic;

namespace BillingIssuance.Api.Application.Abstractions
{
    public interface IResponseConverter
    {
        BillingResponse ToResponse(Billing billing);
        List<BillingResponse> ToResponse(List<Billing> billings);
    }
}
