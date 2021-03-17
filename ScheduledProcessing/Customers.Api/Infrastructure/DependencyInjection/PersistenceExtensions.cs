using Customers.Api.Application.Abstractions;
using Customers.Api.Infrastructure.Persistence;
using Customers.Api.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Customers.Api.Infrastructure.DependencyInjection
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
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IUnitofwork, Unitofwork>();
        }
    }
}
