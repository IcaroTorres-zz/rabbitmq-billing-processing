using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PrivatePackage.Abstractions;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace PrivatePackage.Messaging
{
    public class PublisherRabbitMQ : IMessagePublisher
    {
        private readonly IConnection connection;
        private readonly IRabbitMQSettings settings;
        private readonly ILogger<IMessagePublisher> logger;
        private readonly ConcurrentDictionary<ulong, string> outstandingConfirms;

        public PublisherRabbitMQ(
            IConnection connection,
            IRabbitMQSettings settings,
            ILogger<IMessagePublisher> logger)
        {
            this.connection = connection;
            this.settings = settings;
            this.logger = logger;
            outstandingConfirms = new ConcurrentDictionary<ulong, string>();
        }

        public async Task Publish(MessageBase messageSettings)
        {
            await Task.Run(() =>
            {
                using var channel = connection.CreateModel();
                var exchangeSettings = settings.PublishExchanges.GetSettings(messageSettings.Publisher);
                channel.ExchangeDeclare(exchange: exchangeSettings.Name, type: exchangeSettings.Type);

                var message = JsonConvert.SerializeObject(messageSettings.Payload, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });
                var body = Encoding.UTF8.GetBytes(message);

                messageSettings.ConfigureConfirmation(channel, exchangeSettings.RoutingKey, message, outstandingConfirms, logger);
                channel.BasicPublish(exchange: exchangeSettings.Name,
                                     routingKey: exchangeSettings.RoutingKey,
                                     basicProperties: null,
                                     body: body);
            });
        }
    }
}
