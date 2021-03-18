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
        private readonly IModel _channel;
        private readonly string _targetQueueName;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _properties;
        private readonly BlockingCollection<T> _responseData = new BlockingCollection<T>();

        public RpcClient(IModel channel, string targetQueueName)
        {
            _channel = channel;
            _targetQueueName = targetQueueName;

            _replyQueueName = channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(channel);
            _properties = channel.CreateBasicProperties();
            var customerCorrelationId = Guid.NewGuid().ToString();
            _properties.CorrelationId = customerCorrelationId;
            _properties.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == customerCorrelationId)
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    var data = JsonConvert.DeserializeObject<T>(response);
                    _responseData.Add(data);
                }
            };
        }

        public T CallProcedure<P>(P payload)
        {
            var message = JsonConvert.SerializeObject(payload, typeof(P), default);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "",
                routingKey: _targetQueueName,
                basicProperties: _properties,
                body: messageBytes);

            _channel.BasicConsume(
                queue: _replyQueueName,
                autoAck: true,
                consumer: _consumer);

            return _responseData.Take();
        }
    }
}
