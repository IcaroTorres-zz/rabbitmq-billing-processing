using Customers.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence
{
    public class CustomersContext : DbContext
    {
        public CustomersContext(DbContextOptions<CustomersContext> options) : base(options)
        {
        }

        /// <summary>
        /// For mock purposes
        /// </summary>
        public CustomersContext()
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<NullCustomer>();
            modelBuilder.ApplyConfiguration(new CustomerMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
