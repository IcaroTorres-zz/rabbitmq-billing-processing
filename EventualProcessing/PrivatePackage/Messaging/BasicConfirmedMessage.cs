using Microsoft.Extensions.Logging;
using PrivatePackage.Abstractions;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace PrivatePackage.Messaging
{
    /// <summary>
    /// Pre-configured message enabling publisher acknowledgements
    /// </summary>
    public class BasicConfirmedMessage : MessageBase
    {
        public BasicConfirmedMessage(object payload, string publisherName) : base(payload, publisherName)
        {
        }
        /// <inheritdoc/>
        internal override void ConfigureConfirmation(
            IModel channel,
            string routingKey,
            string body,
            ConcurrentDictionary<ulong, string> outstandingConfirms,
            ILogger<IMessagePublisher> logger)
        {
            channel.ConfirmSelect();
            outstandingConfirms.TryAdd(channel.NextPublishSeqNo, body);
            RegisterEventCallbacks(channel, routingKey, outstandingConfirms, logger);
        }

        private void RegisterEventCallbacks(
            IModel channel, string routingKey,
            ConcurrentDictionary<ulong, string> outstandingConfirms,
            ILogger<IMessagePublisher> logger)
        {
            channel.BasicAcks += (sender, ea) =>
            {
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);
                logger.LogInformation($"Message ack-ed. Publisher: {Publisher}, RoutingKey: {routingKey}, DeliveryTag: {ea.DeliveryTag}, Multiple: {ea.Multiple}, Body: {body}");
                CleanOutstandingConfirms(outstandingConfirms, ea.DeliveryTag, ea.Multiple);
            };
            channel.BasicNacks += (sender, ea) =>
            {
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);
                logger.LogWarning($"Message nack-ed. Publisher: {Publisher}, RoutingKey: {routingKey}, DeliveryTag: {ea.DeliveryTag}, multiple: {ea.Multiple}, body: {body}");
                CleanOutstandingConfirms(outstandingConfirms, ea.DeliveryTag, ea.Multiple);
            };
        }

        private void CleanOutstandingConfirms(ConcurrentDictionary<ulong, string> outstandingConfirms, ulong deliveryTag, bool multiple)
        {
            if (!multiple)
            {
                outstandingConfirms.TryRemove(deliveryTag, out _);
                return;
            }

            foreach (var entry in outstandingConfirms)
                if (entry.Key <= deliveryTag)
                    outstandingConfirms.TryRemove(entry.Key, out _);
        }
    }
}
