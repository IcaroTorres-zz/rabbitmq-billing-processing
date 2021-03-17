using Issuance.Api.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Issuance.Api.Infrastructure.DependencyInjection
{
    public static class DomainExtensions
    {
        public static IServiceCollection BootstrapDomainServices(this IServiceCollection services)
        {
            return services.AddTransient<IModelFactory, ModelFactory>();
        }
    }
}
