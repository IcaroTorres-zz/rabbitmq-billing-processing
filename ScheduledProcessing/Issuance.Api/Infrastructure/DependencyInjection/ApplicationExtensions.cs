using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Services;
using Issuance.Api.Application.Usecases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Library.PipelineBehaviors;
using System.Reflection;

namespace Issuance.Api.Infrastructure.DependencyInjection
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection BootstrapPipelinesServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IResponseConverter, ResponseConverter>()

                // mediatR dependency injection
                .AddMediatR(typeof(IssuanceUsecase).GetTypeInfo().Assembly)

                // mediatR pre-request open-type pipeline behaviors
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidation<,>));
        }
    }
}
