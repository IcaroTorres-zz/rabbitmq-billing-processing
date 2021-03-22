using BillingIssuance.Api.Application.Abstractions;
using BillingIssuance.Api.Domain.Models;
using BillingIssuance.Api.Infrastructure.Persistence.Services;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Infrastructure.Persistence
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IBillingIssuanceContext context;

        public CustomerRepository(IBillingIssuanceContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistEnabledAsync(ulong cpf, CancellationToken token)
        {
            return await context.Customers.Find(QueryFilters.CustomerByCpf(cpf) &
                                                QueryFilters.CustomerActive()).CountDocumentsAsync(token) > 0;
        }

        public async Task InsertOrUpdateAsync(Customer entity, CancellationToken token = default)
        {
            var isInsert = await context.Customers.Find(
                QueryFilters.CustomerByCpf(entity.Cpf)).CountDocumentsAsync(token) == 0;

            if (isInsert) await context.Customers.InsertOneAsync(entity, cancellationToken: token);
            else
            {
                await context.Customers.UpdateOneAsync(
                    QueryFilters.CustomerByCpf(entity.Cpf),
                    Builders<Customer>.Update.Set(x => x.Active, entity.Active),
                    cancellationToken: token);
            }
        }
    }
}
