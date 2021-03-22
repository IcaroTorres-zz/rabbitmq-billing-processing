using BillingProcessing.Api.Domain.Models;
using MongoDB.Driver;

namespace BillingProcessing.Api.Infrastructure.Persistence.Services
{
    public class BillingProcessingContext : IBillingProcessingContext
    {
        private readonly IMongoDatabase database;

        public BillingProcessingContext(IMongoDatabase database, ICollectionsDictionary collectionsDictionary)
        {
            this.database = database;
            Customers = database.GetCollection<Customer>(collectionsDictionary.GetCollectionName(nameof(Customer)));
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Customer> Customers { get; }
        public IMongoCollection<Billing> Billings { get; }
    }
}
