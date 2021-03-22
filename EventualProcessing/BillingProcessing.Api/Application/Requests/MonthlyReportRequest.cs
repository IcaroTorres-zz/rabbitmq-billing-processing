using MediatR;
using PrivatePackage.Abstractions;

namespace BillingProcessing.Api.Application.Requests
{
    public class MonthlyReportRequest : IRequest<IResult>
    {
    }
}
