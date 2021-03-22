using BillingProcessing.Api.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task InsertOrUpdateAsync(Customer customer, CancellationToken token);
        Task<bool> ExistEnabledAsync(ulong v, CancellationToken ct);
        Task<Customer> GetAsync(ulong id, CancellationToken token);
    }
}
