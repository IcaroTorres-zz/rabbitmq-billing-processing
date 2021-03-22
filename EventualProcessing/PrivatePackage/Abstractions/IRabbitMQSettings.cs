using PrivatePackage.Messaging;

namespace PrivatePackage.Abstractions
{
    public interface IRabbitMQSettings
    {
        string AmqpUrl { get; }
        bool DispatchConsumersAsync { get; }
        bool AutomaticRecoveryEnabled { get; }
        ExchangeDictionary PublishExchanges { get; }
        ExchangeDictionary ConsumeExchanges { get; }
    }
}
