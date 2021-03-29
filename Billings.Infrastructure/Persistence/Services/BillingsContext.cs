using Billings.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Billings.Infrastructure.Persistence.Services
{

    public class BillingsContext : IBillingsContext
    {
        public BillingsContext(IMongoDatabase database, CollectionsDictionary collectionsDictionary)
        {
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Billing> Billings { get; }
    }
}
