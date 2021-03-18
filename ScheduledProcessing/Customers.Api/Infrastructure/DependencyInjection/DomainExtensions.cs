using Customers.Api.Domain.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainExtensions
    {
        public static IServiceCollection BootstrapDomainServices(this IServiceCollection services)
        {
            return services.AddScoped<IModelFactory, ModelFactory>();
        }
    }
}
