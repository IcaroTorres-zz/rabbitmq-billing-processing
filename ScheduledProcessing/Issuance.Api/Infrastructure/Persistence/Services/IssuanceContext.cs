using Issuance.Api.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Issuance.Api.Infrastructure.Persistence.Services
{
    public class IssuanceContext : IIssuanceContext
    {
        private readonly IMongoDatabase database;

        public IssuanceContext(IMongoDatabase database, CollectionsDictionary collectionsDictionary)
        {
            this.database = database;
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Billing> Billings { get; }
    }
}
