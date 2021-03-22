using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Responses;
using BillingProcessing.Api.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingProcessing.Api.Application.Services
{
    public class ResponseConverter : IResponseConverter
    {
        public CustomerReportResponse ToResponse(List<Billing> billings, DateTime startDate, DateTime EndDate)
        {
            return new CustomerReportResponse
            {
                Cpf = billings.FirstOrDefault()?.Cpf.ToString().PadLeft(11, '0'),
                StartDate = $"{startDate:dd-MM-yyyy}",
                EndDate = $"{EndDate:dd-MM-yyyy}",
                Billings = billings.ConvertAll(ToResponse),
                Total = billings.Sum(x => x.Amount),
            };
        }

        public BillingResponse ToResponse(Billing charge)
        {
            return new BillingResponse
            {
                Amount = charge.Amount,
                DueDate = charge.DueDateTime.ToString("dd-MM-yyy"),
                ProcessedAt = charge.ProcessedAt.Value.ToString("dd-MM-yyyy")
            };
        }
    }
}
