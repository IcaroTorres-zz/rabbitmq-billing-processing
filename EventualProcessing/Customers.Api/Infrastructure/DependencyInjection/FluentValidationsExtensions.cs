using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PrivatePackage.Abstractions;
using PrivatePackage.Validators;
using System.Reflection;

namespace Customers.Api.Infrastructure.DependencyInjection
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
