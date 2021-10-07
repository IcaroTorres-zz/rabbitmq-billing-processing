using Customers.Application.Workers;
using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DependencyInjection
{

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
