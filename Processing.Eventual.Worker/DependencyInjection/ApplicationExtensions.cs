using Library.PipelineBehaviors;
using MediatR;
using Processing.Eventual.Domain.Services;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection BootstrapPipelinesServices(this IServiceCollection services)
        {
            return services
                // mediatR dependency injection
                .AddTransient<IAmountProcessor, MathOnlyAmountProcessor>()
                .AddMediatR(Assembly.GetExecutingAssembly())

                // mediatR pre-request open-type pipeline behaviors
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidation<,>));
        }
    }
}
