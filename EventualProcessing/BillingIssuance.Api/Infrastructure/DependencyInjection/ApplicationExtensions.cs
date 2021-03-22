using BillingIssuance.Api.Application.Abstractions;
using BillingIssuance.Api.Application.Services;
using BillingIssuance.Api.Application.Usecases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PrivatePackage.PipelineBehaviors;
using System.Reflection;

namespace BillingIssuance.Api.Infrastructure.DependencyInjection
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection BootstrapPipelinesServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IResponseConverter, ResponseConverter>()

                // mediatR dependency injection
                .AddMediatR(typeof(BillingIssuanceUsecase).GetTypeInfo().Assembly)

                // mediatR pre-request open-type pipeline behaviors
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidation<,>));
        }
    }
}
