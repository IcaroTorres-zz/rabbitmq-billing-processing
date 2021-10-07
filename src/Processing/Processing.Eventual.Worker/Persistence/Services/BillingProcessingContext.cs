using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Worker.Persistence.Services
{
    public class BillingProcessingContext : IBillingProcessingContext
    {
        public BillingProcessingContext(IMongoDatabase database, CollectionsDictionary collectionsDictionary)
        {
            Customers = database.GetCollection<Customer>(collectionsDictionary.GetCollectionName(nameof(Customer)));
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Customer> Customers { get; }
        public IMongoCollection<Billing> Billings { get; }
    }
}
