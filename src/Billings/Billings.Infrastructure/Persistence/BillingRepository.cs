using Billings.Application.Abstractions;
using Billings.Domain.Models;
using Billings.Infrastructure.Persistence.Services;
using Library.Results;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Billings.Infrastructure.Persistence
{
    public class BillingRepository : IBillingRepository
    {
        private readonly IBillingsContext _context;

        public BillingRepository(IBillingsContext context)
        {
            _context = context;
        }

        public async Task InsertAsync(Billing entity, CancellationToken token)
        {
            if (entity is INull) return;
            await _context.Billings.InsertOneAsync(entity, cancellationToken: token);
        }

        public async Task<List<Billing>> GetManyAsync(ulong cpf, byte month, ushort year, CancellationToken token)
        {
            return await _context.Billings.FindSync(
                QueryFilters.ByCustomerCpf(cpf) &
                QueryFilters.ByMonthYear(month, year)).ToListAsync(token);
        }

        public async Task<List<Billing>> GetPendingAsync(CancellationToken token)
        {
            return await _context.Billings.FindSync(QueryFilters.PendingProcessment()).ToListAsync(token);
        }

        public async Task UpdateProcessedBatchAsync(IEnumerable<Billing> entities, CancellationToken token = default)
        {
            var listWrites = new List<WriteModel<Billing>>();

            await Task.Run(() => Parallel.ForEach(entities, entity =>
            {
                if (!(entity is INull))
                {
                    listWrites.Add(new UpdateOneModel<Billing>(
                        QueryFilters.ById(entity.Id), CommandDefinitions.SetProcessed(entity)));
                }
            }), token);

            if (listWrites.Count > 0)
            {
                await _context.Billings.BulkWriteAsync(listWrites, cancellationToken: token);
            }
        }
    }
}
