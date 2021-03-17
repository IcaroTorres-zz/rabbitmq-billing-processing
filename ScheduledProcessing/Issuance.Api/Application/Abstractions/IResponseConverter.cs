using Issuance.Api.Application.Models;
using Issuance.Api.Domain.Models;
using System.Collections.Generic;

namespace Issuance.Api.Application.Abstractions
{
    public interface IResponseConverter
    {
        BillingResponse ToResponse(Billing billing);
        List<BillingResponse> ToResponse(List<Billing> billings);
    }
}
