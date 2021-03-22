using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using BillingProcessing.Api.Infrastructure.Persistence.Services;
using MongoDB.Driver;
using PrivatePackage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Infrastructure.Persistence
{
    public class BillingsRepository : IBillingsRepository
    {
        private readonly IBillingProcessingContext context;

        public BillingsRepository(IBillingProcessingContext context)
        {
            this.context = context;
        }

        public async Task<List<Billing>> GetCustomerPendingBillingsAsync(ulong cpf, CancellationToken token)
        {
            return await context.Billings.FindSync(QueryFilters.BillingsByCustomerCpf(cpf) &
                                                   QueryFilters.BillingsPending()).ToListAsync(token);
        }

        public async Task<List<Billing>> GetCustomerProcessedBillingsAsync(ulong cpf, DateTime startDate, DateTime endDate, CancellationToken token)
        {
            return await context.Billings.FindSync(QueryFilters.BillingsByCustomerCpf(cpf) &
                                                   QueryFilters.BillingsProcessed() &
                                                   QueryFilters.BillingsDueDateInRange(startDate, endDate)).ToListAsync(token);
        }

        public async Task<Billing> InsertAsync(Billing entity, CancellationToken token)
        {
            if (!(entity is INull))
            {
                await context.Billings.InsertOneAsync(entity, cancellationToken: token);
            }
            return entity;
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
                await context.Billings.BulkWriteAsync(listWrites, cancellationToken: token);
            }
        }
    }
}
