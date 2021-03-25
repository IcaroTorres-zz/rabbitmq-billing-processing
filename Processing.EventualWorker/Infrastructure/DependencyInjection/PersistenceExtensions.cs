using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Processing.EventualWorker.Application.Abstractions;
using Processing.EventualWorker.Infrastructure.Persistence;
using Processing.EventualWorker.Infrastructure.Persistence.Services;

namespace Microsoft.Extensions.DependencyInjection
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
                .AddSingleton(collections)
                .AddSingleton<IBillingProcessingContext, BillingProcessingContext>()
                .AddSingleton<ICustomerRepository, CustomerRepository>()
                .AddSingleton<IBillingsRepository, BillingsRepository>();
        }
    }
}
