using Billings.Application.Abstractions;
using Billings.Infrastructure.Persistence;
using Billings.Infrastructure.Persistence.Services;
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
                .AddSingleton(mongoDB.Collections)
                .AddSingleton<IBillingsContext, BillingsContext>()
                .AddSingleton<IBillingRepository, BillingRepository>();
        }
    }
}
