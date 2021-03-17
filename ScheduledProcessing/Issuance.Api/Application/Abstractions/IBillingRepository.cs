using Issuance.Api.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Abstractions
{
    public interface IBillingRepository
    {
        Task InsertAsync(Billing entity, CancellationToken token);
        Task<List<Billing>> GetManyAsync(ulong cpf, byte month, ushort year, CancellationToken token);
        Task<List<Billing>> GetPendingAsync(CancellationToken token);
        Task UpdateProcessedBatchAsync(IEnumerable<Billing> processedBillings, CancellationToken token = default);
    }
}
