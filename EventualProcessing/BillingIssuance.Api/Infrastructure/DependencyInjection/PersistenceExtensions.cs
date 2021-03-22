using BillingIssuance.Api.Application.Abstractions;
using BillingIssuance.Api.Infrastructure.Persistence;
using BillingIssuance.Api.Infrastructure.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace BillingIssuance.Api.Infrastructure.DependencyInjection
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection BootstrapPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            var mongoConnectionString = config["MongoDB:ConnectionString"];
            var database = config["MongoDB:DatabaseName"];
            var collections = config.GetSection("MongoDB:Collections").Get<CollectionsDictionary>();

            return services
                .AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(mongoConnectionString))
                .AddSingleton<IMongoDatabase>(x => x.GetRequiredService<IMongoClient>().GetDatabase(database))
                .AddSingleton<ICollectionsDictionary, CollectionsDictionary>(_ => collections)
                .AddSingleton<IBillingIssuanceContext, BillingIssuanceContext>()
                .AddSingleton<IBillingRepository, BillingRepository>()
                .AddSingleton<ICustomerRepository, CustomerRepository>();
        }
    }
}
