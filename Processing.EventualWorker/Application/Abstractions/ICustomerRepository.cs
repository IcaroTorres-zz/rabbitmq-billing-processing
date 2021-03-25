using Processing.EventualWorker.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.EventualWorker.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task InsertAsync(Customer entity, CancellationToken token);
        Task<Customer> GetAsync(ulong cpf, CancellationToken token);
    }
}
