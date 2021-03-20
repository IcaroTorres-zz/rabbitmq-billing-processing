using Issuance.Api.Application.Abstractions;
using Issuance.Api.Infrastructure.Persistence;
using Issuance.Api.Infrastructure.Persistence.Services;
using MongoDB.Driver;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection BootstrapPersistenceServices(this IServiceCollection services, MongoDBSettings mongoDB)
        {
            return services
                .AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(mongoDB.ConnectionString))
                .AddSingleton<IMongoDatabase>(x => x.GetRequiredService<IMongoClient>().GetDatabase(mongoDB.DatabaseName))
                .AddSingleton(_ => mongoDB.Collections)
                .AddSingleton<IIssuanceContext, IssuanceContext>()
                .AddSingleton<IBillingRepository, BillingRepository>();
        }
    }
}
