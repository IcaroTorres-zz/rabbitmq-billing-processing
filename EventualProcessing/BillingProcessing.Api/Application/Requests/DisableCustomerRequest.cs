using MediatR;
using PrivatePackage.Abstractions;

namespace BillingProcessing.Api.Application.Requests
{
    public class DisableCustomerRequest : IRequest<IResult>
    {
        public string CpfString { get; set; }
        public ulong CpfLong { get; set; }
    }
}