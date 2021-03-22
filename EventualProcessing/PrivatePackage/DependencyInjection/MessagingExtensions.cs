using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrivatePackage.Abstractions;
using PrivatePackage.Messaging;
using RabbitMQ.Client;
using System;

namespace PrivatePackage.DependencyInjection
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
                .AddSingleton<IRabbitMQSettings, RabbitMQSettings>(_ => settings)
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
