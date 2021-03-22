using Customers.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customers.Api.Infrastructure.Persistence
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> modelBuilder)
        {
            modelBuilder.HasKey(x => x.Cpf);
            modelBuilder.HasIndex(u => u.Cpf).IsUnique();
            modelBuilder.Property(x => x.Name).IsRequired();
            modelBuilder.Property(x => x.State).IsRequired();
        }
    }
}
