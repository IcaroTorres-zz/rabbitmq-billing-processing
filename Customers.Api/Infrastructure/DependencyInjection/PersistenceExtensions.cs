using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Workers;
using Customers.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class PersistenceExtensions
    {
        public static IServiceCollection BootstrapPersistenceServices(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            return services
                .AddHostedService<ScheduledCustomerAcceptProcessWorker>()
                .AddDbContext<CustomersContext>(options =>
                {
                    var sourcePath = Path.Combine(env.ContentRootPath, ".\\Infrastructure\\Persistence", configuration["SQLite:DatabaseName"]);
                    options.UseSqlite($"Data Source={sourcePath}");
                    if (!env.IsProduction()) options.EnableSensitiveDataLogging();
                })
                .AddSingleton<ICustomerRepositoryFactory>(x =>
                {
                    var dbOptions = x.CreateScope()
                        .ServiceProvider
                        .GetRequiredService<DbContextOptions<CustomersContext>>();
                    return new CustomerRepositoryFactory(dbOptions);
                })
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IUnitofwork, Unitofwork>();
        }
    }
}
