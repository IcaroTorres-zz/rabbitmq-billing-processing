using BillingProcessing.Api.Application.Responses;
using BillingProcessing.Api.Domain.Models;
using System;
using System.Collections.Generic;

namespace BillingProcessing.Api.Application.Abstractions
{
    public interface IResponseConverter
    {
        CustomerReportResponse ToResponse(List<Billing> charges, DateTime startDate, DateTime EndDate);
    }
}
