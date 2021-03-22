using BillingIssuance.Api.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task<bool> ExistEnabledAsync(ulong cpf, CancellationToken ct);
        Task InsertOrUpdateAsync(Customer customer, CancellationToken token);
    }
}
