using Issuance.Api.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Issuance.Api.Infrastructure.Persistence.Services
{
    [ExcludeFromCodeCoverage]
    public class IssuanceContext : IIssuanceContext
    {
        public IssuanceContext(IMongoDatabase database, CollectionsDictionary collectionsDictionary)
        {
            Billings = database.GetCollection<Billing>(collectionsDictionary.GetCollectionName(nameof(Billing)));
        }

        public IMongoCollection<Billing> Billings { get; }
    }
}
