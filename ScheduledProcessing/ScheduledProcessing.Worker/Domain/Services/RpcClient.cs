using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace ScheduledProcessing.Worker.Domain.Services
{
    public class RpcClient<T> : IRpcClient<T>
    {
        private readonly IModel channel;
        private readonly string targetQueueName;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly IBasicProperties properties;
        private readonly BlockingCollection<T> responseData = new BlockingCollection<T>();

        public RpcClient(IModel channel, string targetQueueName)
        {
            this.channel = channel;
            this.targetQueueName = targetQueueName;

            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            properties = channel.CreateBasicProperties();
            var customerCorrelationId = Guid.NewGuid().ToString();
            properties.CorrelationId = customerCorrelationId;
            properties.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == customerCorrelationId)
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    var data = JsonConvert.DeserializeObject<T>(response);
                    responseData.Add(data);
                }
            };
        }

        public T CallProcedure<P>(P payload)
        {
            var message = JsonConvert.SerializeObject(payload, typeof(P), default);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: targetQueueName,
                basicProperties: properties,
                body: messageBytes);

            channel.BasicConsume(
                queue: replyQueueName,
                autoAck: true,
                consumer: consumer);

            return responseData.Take();
        }
    }
}
