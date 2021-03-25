using Issuance.Api.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Issuance.Api.Infrastructure.Persistence.Services
{

    public class IssuanceContext : IIssuanceContext
    {
        public IssuanceContext(IMongoDatabase database, CollectionsDictionary collectionsDictionary)
        {
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Billing> Billings { get; }
    }
}
