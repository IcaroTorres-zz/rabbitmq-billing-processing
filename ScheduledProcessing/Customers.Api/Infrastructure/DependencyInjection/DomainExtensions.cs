using Customers.Api.Domain.Services;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DomainExtensions
    {
        public static IServiceCollection BootstrapDomainServices(this IServiceCollection services)
        {
            return services.AddScoped<IModelFactory, ModelFactory>();
        }
    }
}
