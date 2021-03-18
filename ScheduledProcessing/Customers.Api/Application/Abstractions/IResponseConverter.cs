using Customers.Api.Application.Responses;
using Customers.Api.Domain.Models;

namespace Issuance.Api.Application.Abstractions
{
    public interface IResponseConverter
    {
        CustomerResponse ToResponse(Customer billing);
    }
}
