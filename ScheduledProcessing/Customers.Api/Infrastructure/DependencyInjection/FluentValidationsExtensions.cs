using FluentValidation;
using Library.Validators;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class FluentValidationsExtensions
    {
        public static IServiceCollection BootstrapValidators(this IServiceCollection services)
        {
            return services.AddTransient<ICpfValidator, CpfValidator>()
                           .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
