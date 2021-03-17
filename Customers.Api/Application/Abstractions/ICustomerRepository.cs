using Customers.Api.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task<bool> ExistAsync(ulong id, CancellationToken token);
        Task<Customer> GetAsync(ulong id, CancellationToken token);
        Task InsertAsync(Customer entity, CancellationToken token);
        Task<List<Customer>> GetAllAsync();
    }
}
