using BillingIssuance.Api.Domain.Models;
using MongoDB.Driver;

namespace BillingIssuance.Api.Infrastructure.Persistence.Services
{
    public interface IBillingIssuanceContext
    {
        IMongoCollection<Billing> Billings { get; }
        IMongoCollection<Customer> Customers { get; }
    }
}
