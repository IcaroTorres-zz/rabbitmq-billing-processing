using BillingIssuance.Api.Application.Abstractions;
using BillingIssuance.Api.Application.Models;
using BillingIssuance.Api.Domain.Models;
using System.Collections.Generic;

namespace BillingIssuance.Api.Application.Services
{
    public class ResponseConverter : IResponseConverter
    {
        public BillingResponse ToResponse(Billing billing)
        {
            return new BillingResponse
            {
                Id = billing.Id,
                Amount = billing.Amount,
                Cpf = billing.Cpf.ToString().PadLeft(11, '0'),
                DueDate = $"{billing.DueDate.Day:00}-{billing.DueDate.Month:00}-{billing.DueDate.Year}"
            };
        }

        public List<BillingResponse> ToResponse(List<Billing> billings) => billings.ConvertAll(c => ToResponse(c));
    }
}
