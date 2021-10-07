using Processing.Eventual.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.Eventual.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task InsertAsync(Customer entity, CancellationToken token);
        Task<Customer> GetAsync(ulong cpf, CancellationToken token);
    }
}
