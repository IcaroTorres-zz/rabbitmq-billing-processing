using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Infrastructure.Persistence;
using BillingProcessing.Api.Infrastructure.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace BillingProcessing.Api.Infrastructure.DependencyInjection
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
                .AddSingleton<IBillingProcessingContext, BillingProcessingContext>()
                .AddSingleton<ICustomerRepository, CustomerRepository>()
                .AddSingleton<IBillingsRepository, BillingsRepository>()
                .AddSingleton<IMonthlyReportRepository, MonthlyReportRepository>();
        }
    }
}
