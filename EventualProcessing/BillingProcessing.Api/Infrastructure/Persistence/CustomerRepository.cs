using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using BillingProcessing.Api.Infrastructure.Persistence.Services;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Infrastructure.Persistence
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IBillingProcessingContext context;

        public CustomerRepository(IBillingProcessingContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistEnabledAsync(ulong cpf, CancellationToken token)
        {
            return await context.Customers.Find(QueryFilters.CustomerByCpf(cpf) &
                                                QueryFilters.CustomerActive()).CountDocumentsAsync(token) > 0;
        }

        public async Task<Customer> GetAsync(ulong cpf, CancellationToken token)
        {
            return await context.Customers.Find(QueryFilters.CustomerByCpf(cpf))
                                          .FirstOrDefaultAsync(token) ?? Customer.Null;
        }

        public async Task InsertOrUpdateAsync(Customer entity, CancellationToken token)
        {
            if (await context.Customers.Find(QueryFilters.CustomerByCpf(entity.Cpf)).CountDocumentsAsync(token) > 0)
            {
                await context.Customers.UpdateOneAsync(
                    QueryFilters.CustomerByCpf(entity.Cpf),
                    CommandDefinitions.SetEnabled(entity), cancellationToken: token);
            }
            else await context.Customers.InsertOneAsync(entity, cancellationToken: token);
        }
    }
}
