using Customers.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api.Infrastructure.Persistence
{
    public class CustomersContext : DbContext
    {
        public CustomersContext(DbContextOptions<CustomersContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<NullCustomer>();
            builder.ApplyConfiguration(new CustomerMap());
            base.OnModelCreating(builder);
        }
    }
}
