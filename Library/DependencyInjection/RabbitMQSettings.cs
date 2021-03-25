using Library.Messaging;

namespace Microsoft.Extensions.DependencyInjection
{
    public sealed class RabbitMQSettings
    {
        public string AmqpUrl { get; set; }
        public bool DispatchConsumersAsync { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public ExchangeDictionary PublishExchanges { get; set; }
        public ExchangeDictionary ConsumeExchanges { get; set; }
    }
}
