using BillingIssuance.Api.Domain.Models;
using MongoDB.Driver;

namespace BillingIssuance.Api.Infrastructure.Persistence.Services
{
    public class BillingIssuanceContext : IBillingIssuanceContext
    {
        public BillingIssuanceContext(IMongoDatabase database, ICollectionsDictionary collectionsDictionary)
        {
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
            Customers = database.GetCollection<Customer>(collectionsDictionary.GetCollectionName(nameof(Customer)));
        }

        public IMongoCollection<Billing> Billings { get; }
        public IMongoCollection<Customer> Customers { get; }
    }
}
