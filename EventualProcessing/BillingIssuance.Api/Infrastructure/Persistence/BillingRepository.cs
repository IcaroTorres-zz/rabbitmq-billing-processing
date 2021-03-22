using BillingIssuance.Api.Application.Abstractions;
using BillingIssuance.Api.Domain.Models;
using BillingIssuance.Api.Infrastructure.Persistence.Services;
using MongoDB.Driver;
using PrivatePackage.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Infrastructure.Persistence
{
    public class BillingRepository : IBillingRepository
    {
        private readonly IBillingIssuanceContext context;

        public BillingRepository(IBillingIssuanceContext context)
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
    }
}
