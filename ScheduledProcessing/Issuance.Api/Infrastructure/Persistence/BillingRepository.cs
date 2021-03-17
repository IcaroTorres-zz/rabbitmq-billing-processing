using Issuance.Api.Application.Abstractions;
using Issuance.Api.Domain.Models;
using Issuance.Api.Infrastructure.Persistence.Services;
using MongoDB.Driver;
using Library.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Infrastructure.Persistence
{
    public class BillingRepository : IBillingRepository
    {
        private readonly IIssuanceContext context;

        public BillingRepository(IIssuanceContext context)
        {
            this.context = context;
        }

        public async Task InsertAsync(Billing entity, CancellationToken token)
        {
            if (entity is INull) return;
            await context.Billings.InsertOneAsync(entity, cancellationToken: token);
        }

        public async Task<List<Billing>> GetManyAsync(ulong cpf, byte month, ushort year, CancellationToken token)
        {
            return await context.Billings.FindSync(QueryFilters.ByCustomerCpf(cpf) &
                                                   QueryFilters.ByMonthYear(month, year)).ToListAsync(token);
        }

        public async Task<List<Billing>> GetPendingAsync(CancellationToken token)
        {
            return await context.Billings.FindSync(QueryFilters.PendingProcessment()).ToListAsync(token);
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
                await context.Billings.BulkWriteAsync(listWrites, cancellationToken: token);
            }
        }
    }
}
