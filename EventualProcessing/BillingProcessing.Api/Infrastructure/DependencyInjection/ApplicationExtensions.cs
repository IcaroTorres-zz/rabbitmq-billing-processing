using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Services;
using BillingProcessing.Api.Application.Usecases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PrivatePackage.PipelineBehaviors;
using System.Reflection;

namespace BillingProcessing.Api.Infrastructure.DependencyInjection
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection BootstrapPipelinesServices(this IServiceCollection services)
        {
            return services
                // mediatR dependency injection
                .AddTransient<IAmountCalculator, MathOnlyAmountCalculator>()
                .AddTransient<IResponseConverter, ResponseConverter>()
                .AddMediatR(typeof(CustomerReportUsecase).GetTypeInfo().Assembly)

                // mediatR pre-request open-type pipeline behaviors
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidation<,>));
        }
    }
}
