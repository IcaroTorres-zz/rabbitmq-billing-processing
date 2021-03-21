using Customers.Api.Application.Workers;
using RabbitMQ.Client;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class WorkerExtensions
    {
        public static IServiceCollection BootstrapWorkerServices(this IServiceCollection services, RabbitMQSettings rabbitMQ)
        {
            return services
                .AddSingleton(rabbitMQ)
                .AddSingleton<IConnectionFactory>(_ => new ConnectionFactory { Uri = new Uri(rabbitMQ.AmqpUrl) })
                .AddHostedService<ScheduledCustomerAcceptProcessWorker>();
        }
    }
}
