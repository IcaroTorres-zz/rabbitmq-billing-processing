using Customers.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Customers.Api.Infrastructure.Persistence
{
    [ExcludeFromCodeCoverage]
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Cpf);
            builder.HasIndex(u => u.Cpf).IsUnique();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.State).IsRequired();
        }
    }
}
