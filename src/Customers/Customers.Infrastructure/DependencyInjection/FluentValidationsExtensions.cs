using FluentValidation;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class FluentValidationsExtensions
    {
        public static IServiceCollection BootstrapValidators(this IServiceCollection services)
        {
            return services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
