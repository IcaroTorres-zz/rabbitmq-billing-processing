using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Processing.EventualWorker.Domain.Models;

namespace Processing.EventualWorker.Infrastructure.Persistence.Services
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
