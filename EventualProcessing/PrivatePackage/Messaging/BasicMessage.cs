using Microsoft.Extensions.Logging;
using PrivatePackage.Abstractions;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace PrivatePackage.Messaging
{
    /// <summary>
    /// Message without publisher acknowledgements
    /// </summary>
    public class BasicMessage : MessageBase
    {
        public BasicMessage(object payload, string publisher) : base(payload, publisher)
        {
        }
        /// <inheritdoc/>
        internal override void ConfigureConfirmation(IModel channel, string routingKey, string message, ConcurrentDictionary<ulong, string> outstandingConfirms, ILogger<IMessagePublisher> logger)
        {
        }
    }
}
