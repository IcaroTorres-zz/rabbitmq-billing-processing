using BillingIssuance.Api.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task<bool> ExistEnabledAsync(ulong cpf, CancellationToken token);
        Task InsertOrUpdateAsync(Customer entity, CancellationToken token);
    }
}
