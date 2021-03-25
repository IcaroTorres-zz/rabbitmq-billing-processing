using MongoDB.Driver;
using Processing.EventualWorker.Application.Abstractions;
using Processing.EventualWorker.Domain.Models;
using Processing.EventualWorker.Infrastructure.Persistence.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.EventualWorker.Infrastructure.Persistence
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IBillingProcessingContext _context;

        public CustomerRepository(IBillingProcessingContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetAsync(ulong cpf, CancellationToken token)
        {
            return await _context.Customers.Find(QueryFilters.CustomerByCpf(cpf))
                                        .FirstOrDefaultAsync(token) ?? Customer.Null;
        }

        public async Task InsertAsync(Customer entity, CancellationToken token)
        {
            await _context.Customers.InsertOneAsync(entity, cancellationToken: token);
        }
    }
}
