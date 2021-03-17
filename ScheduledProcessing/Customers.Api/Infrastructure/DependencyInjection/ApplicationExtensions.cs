using Customers.Api.Application.Usecases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Library.PipelineBehaviors;
using System.Reflection;

namespace Customers.Api.Infrastructure.DependencyInjection
{
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
