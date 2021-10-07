using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace Library.Messaging
{
    public class PublisherRabbitMQ : IMessagePublisher
    {
        private readonly IConnection _connection;
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<IMessagePublisher> _logger;
        private readonly ConcurrentDictionary<ulong, string> _outstandingConfirms;

        public PublisherRabbitMQ(
            IConnection connection,
            RabbitMQSettings settings,
            ILogger<IMessagePublisher> logger)
        {
            _connection = connection;
            _settings = settings;
            _logger = logger;
            _outstandingConfirms = new ConcurrentDictionary<ulong, string>();
        }

        public async Task Publish(MessageBase messageSettings)
        {
            await Task.Run(() =>
            {
                using var channel = _connection.CreateModel();
                var exchangeSettings = _settings.PublishExchanges.GetSettings(messageSettings.Publisher);
                channel.ExchangeDeclare(exchange: exchangeSettings.Name, type: exchangeSettings.Type);

                var message = JsonConvert.SerializeObject(messageSettings.Payload, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });
                var body = Encoding.UTF8.GetBytes(message);

                messageSettings.ConfigureConfirmation(channel, exchangeSettings.RoutingKey, message, _outstandingConfirms, _logger);
                channel.BasicPublish(exchange: exchangeSettings.Name,
                                    routingKey: exchangeSettings.RoutingKey,
                                    basicProperties: null,
                                    body: body);
            });
        }
    }
}
