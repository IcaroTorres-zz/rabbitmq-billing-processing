using BillingProcessing.Api.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task InsertOrUpdateAsync(Customer entity, CancellationToken token);
        Task<bool> ExistEnabledAsync(ulong cpf, CancellationToken token);
        Task<Customer> GetAsync(ulong cpf, CancellationToken token);
    }
}
