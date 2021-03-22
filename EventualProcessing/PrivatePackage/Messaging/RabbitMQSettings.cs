using PrivatePackage.Abstractions;

namespace PrivatePackage.Messaging
{
    public sealed class RabbitMQSettings : IRabbitMQSettings
    {
        public string AmqpUrl { get; set; }
        public bool DispatchConsumersAsync { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public ExchangeDictionary PublishExchanges { get; set; }
        public ExchangeDictionary ConsumeExchanges { get; set; }
    }
}
