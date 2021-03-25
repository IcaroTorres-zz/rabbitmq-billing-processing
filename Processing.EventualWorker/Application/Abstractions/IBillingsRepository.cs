using Processing.EventualWorker.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.EventualWorker.Application.Abstractions
{
    public interface IBillingsRepository
    {
        Task<List<Billing>> GetCustomerPendingBillingsAsync(ulong cpf, CancellationToken token);
        Task InsertAsync(Billing entity, CancellationToken token);
        Task UpdateManyProcessedAsync(IEnumerable<Billing> entities, CancellationToken token);
        Task RemoveManyConfirmedAsync(IEnumerable<Billing> entities, CancellationToken token);
    }
}
