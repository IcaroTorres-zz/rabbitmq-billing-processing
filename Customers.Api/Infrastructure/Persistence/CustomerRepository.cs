using Customers.Api.Application.Abstractions;
using Customers.Api.Domain.Models;
using Library.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Infrastructure.Persistence
{
    /// <inheritdoc cref="ICustomerRepository"/>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomersContext _context;

        public CustomerRepository(CustomersContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistAsync(ulong cpf, CancellationToken token)
        {
            return await _context.Customers
                .AsNoTrackingWithIdentityResolution()
                .AnyAsync(x => x.Cpf.Equals(cpf));
        }

        public async Task<Customer> GetAsync(ulong cpf, CancellationToken token)
        {
            return await _context.Customers
                .AsNoTrackingWithIdentityResolution()
                .SingleOrDefaultAsync(x => x.Cpf.Equals(cpf)) ?? Customer.Null;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.AsNoTrackingWithIdentityResolution().ToListAsync();
        }

        public async Task InsertAsync(Customer entity, CancellationToken token)
        {
            if (entity is INull) return;
            await _context.Customers.AddAsync(entity, token);
        }
    }
}
