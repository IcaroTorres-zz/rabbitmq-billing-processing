using Library.Results;
using MongoDB.Driver;
using Processing.Eventual.Application.Abstractions;
using Processing.Eventual.Domain.Models;
using Processing.Eventual.Worker.Persistence.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.Eventual.Worker.Persistence
{
    public class BillingsRepository : IBillingsRepository
    {
        private readonly IBillingProcessingContext _context;

        public BillingsRepository(IBillingProcessingContext context)
        {
            _context = context;
        }

        public async Task<List<Billing>> GetCustomerPendingBillingsAsync(ulong cpf, CancellationToken token)
        {
            return await _context.Billings.FindSync(QueryFilters.BillingsByCustomerCpf(cpf) &
                                                 QueryFilters.BillingsPending()).ToListAsync(token);
        }

        public async Task InsertAsync(Billing entity, CancellationToken token)
        {
            if (!(entity is INull))
            {
                await _context.Billings.InsertOneAsync(entity, cancellationToken: token);
            }
        }

        public async Task UpdateManyProcessedAsync(IEnumerable<Billing> entities, CancellationToken token)
        {
            var eligibleEntities = entities.Where(x => !(x is INull)).ToList();
            if (eligibleEntities.Count > 0)
            {
                var listWrites = new List<WriteModel<Billing>>();
                await Task.Run(() => Parallel.ForEach(eligibleEntities, entity =>
                {
                    listWrites.Add(new UpdateOneModel<Billing>(
                        QueryFilters.BillingById(entity.Id),
                        CommandDefinitions.SetProcessed(entity)));
                }));
                await _context.Billings.BulkWriteAsync(listWrites, cancellationToken: token);
            }
        }

        public async Task RemoveManyConfirmedAsync(IEnumerable<Billing> entities, CancellationToken token)
        {
            var eligibleEntitiesIds = entities.Where(x => !(x is INull)).Select(x => x.Id).ToList();
            await _context.Billings.DeleteManyAsync(QueryFilters.BillingIdIn(eligibleEntitiesIds), cancellationToken: token);
        }
    }
}
