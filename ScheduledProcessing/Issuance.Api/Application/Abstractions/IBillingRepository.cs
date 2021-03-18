using Issuance.Api.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Abstractions
{
    /// <summary>
    /// A null-free memory abstraction for I/O database operations related to <see cref="Billing"/>
    /// </summary>
    public interface IBillingRepository
    {
        /// <summary>
        /// Inserts a new record to database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task InsertAsync(Billing entity, CancellationToken token);

        /// <summary>
        /// Gets a list of <see cref="Billing"/> records from database filtered by cpf and/or month and year.
        /// </summary>
        /// <returns></returns>
        Task<List<Billing>> GetManyAsync(ulong cpf, byte month, ushort year, CancellationToken token);

        /// <summary>
        /// Gets all <see cref="Billing"/> records pending Its amount processing
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<Billing>> GetPendingAsync(CancellationToken token);

        /// <summary>
        /// Updates a batch of processed <see cref="Billing"/> records at once
        /// </summary>
        /// <param name="processedBillings"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UpdateProcessedBatchAsync(IEnumerable<Billing> processedBillings, CancellationToken token = default);
    }
}
