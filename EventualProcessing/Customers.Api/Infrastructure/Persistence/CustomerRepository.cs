﻿using Customers.Api.Application.Abstractions;
using Customers.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using PrivatePackage.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Infrastructure.Persistence
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomersContext context;

        public CustomerRepository(CustomersContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistAsync(ulong id, CancellationToken token)
        {
            return await context.Customers
                .AsNoTrackingWithIdentityResolution()
                .AnyAsync(x => x.Cpf.Equals(id));
        }

        public async Task<Customer> GetAsync(ulong id, CancellationToken token)
        {
            return await context.Customers
                .AsNoTrackingWithIdentityResolution()
                .SingleOrDefaultAsync(x => x.Cpf.Equals(id)) ?? Customer.Null;
        }

        public async Task InsertAsync(Customer entity, CancellationToken token)
        {
            if (entity is INull) return;
            await context.Customers.AddAsync(entity, token);
        }
    }
}