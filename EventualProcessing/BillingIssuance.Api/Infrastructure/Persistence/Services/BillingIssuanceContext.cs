using BillingIssuance.Api.Domain.Models;
using MongoDB.Driver;

namespace BillingIssuance.Api.Infrastructure.Persistence.Services
{
    public class BillingIssuanceContext : IBillingIssuanceContext
    {
        private readonly IMongoDatabase database;

        public BillingIssuanceContext(IMongoDatabase database, ICollectionsDictionary collectionsDictionary)
        {
            this.database = database;
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
            Customers = database.GetCollection<Customer>(collectionsDictionary.GetCollectionName(nameof(Customer)));
        }

        public IMongoCollection<Billing> Billings { get; }
        public IMongoCollection<Customer> Customers { get; }
    }
}
