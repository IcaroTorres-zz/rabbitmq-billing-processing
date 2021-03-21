using Customers.Api.Application.Usecases;
using Library.PipelineBehaviors;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationExtensions
    {
        public static IServiceCollection BootstrapPipelinesServices(this IServiceCollection services)
        {
            return services
                // mediatR dependency injection
                .AddMediatR(typeof(RegisterCustomerUsecase).GetTypeInfo().Assembly)

                // mediatR pre-request open-type pipeline behaviors
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidation<,>));
        }
    }
}
