using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Library.Abstractions;
using Library.Validators;
using System.Reflection;

namespace Issuance.Api.Infrastructure.DependencyInjection
{
    public static class FluentValidationsExtensions
    {
        public static IServiceCollection BootstrapValidators(this IServiceCollection services)
        {
            return services.AddTransient<ICpfValidator, CpfValidator>()
                           .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
