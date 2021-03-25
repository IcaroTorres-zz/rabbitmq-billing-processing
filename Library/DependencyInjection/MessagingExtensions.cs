using Library.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessagingExtensions
    {
        public static IServiceCollection BootstrapMessagingServices(this IServiceCollection services, RabbitMQSettings settings)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(settings.AmqpUrl),
                DispatchConsumersAsync = settings.DispatchConsumersAsync,
                AutomaticRecoveryEnabled = settings.AutomaticRecoveryEnabled
            };
            services
                .AddSingleton(settings)
                .AddSingleton<IConnectionFactory, ConnectionFactory>(_ => factory);

            if (settings.PublishExchanges != null)
            {
                services.AddSingleton<IMessagePublisher, PublisherRabbitMQ>(x =>
                {
                    var connection = factory.CreateConnection();
                    var logger = x.GetRequiredService<ILogger<PublisherRabbitMQ>>();
                    return new PublisherRabbitMQ(connection, settings, logger);
                });
            }
            if (settings.ConsumeExchanges != null)
            {
                services.AddSingleton(_ => factory.CreateConnection())
                       .AddSingleton(typeof(IMessageConsumer<>), typeof(ConsumerRabbitMQ<>));
            }

            return services;
        }
    }
}
