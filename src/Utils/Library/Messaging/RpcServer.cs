using Library.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Messaging
{
    public abstract class RpcServer<T> : BackgroundService, IRpcServer<T> where T : IRpcServer<T>
    {
        public EventHandler<BasicDeliverEventArgs> OnMessageReceived => OnMessageReceivedEventHandler;
        protected readonly EventingBasicConsumer _consumer;
        protected readonly IModel _channel;
        protected readonly ILogger<T> _logger;

        protected RpcServer(string queueName, IConnectionFactory connectionFactory, ILogger<T> logger)
        {
            (_consumer, _channel) = BuildConsumerAndChanel(queueName, connectionFactory);
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Received += OnMessageReceived;
            return Task.CompletedTask;
        }

        protected async void OnMessageReceivedEventHandler(object model, BasicDeliverEventArgs ea)
        {
            OnMessageReceivedStarts(ea);
            string response = string.Empty;
            IBasicProperties replyProperties = CreateBasicProperties(ea);
            try
            {
                OnMessageReaded(ea, await HandleReceivedMessage(ea));
                response = await WriteResponseMessage();
                OnResponseWritten(ea, response);
            }
            catch (Exception ex) { OnMessageReceivedException(ea, ex); }
            finally { OnMessageReceivedEnds(ea, _channel, response, ea.BasicProperties, replyProperties); }
        }

        public virtual (EventingBasicConsumer, IModel) BuildConsumerAndChanel(string queueName, IConnectionFactory connectionFactory)
        {
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine($"Awaiting RPC requests for queue {queueName}");
            return (consumer, channel);
        }

        public virtual IBasicProperties CreateBasicProperties(BasicDeliverEventArgs ea)
        {
            var replyProperties = _channel.CreateBasicProperties();
            replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;
            return replyProperties;
        }

        public abstract Task<string> HandleReceivedMessage(BasicDeliverEventArgs ea);

        public abstract Task<string> WriteResponseMessage();

        protected virtual void OnMessageReceivedStarts(BasicDeliverEventArgs ea)
        {
            _logger.LogInformation($"Received on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}.");
        }

        protected virtual void OnMessageReaded(BasicDeliverEventArgs ea, string receivedMessage)
        {
            _logger.LogInformation($"Readed on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}, Body: {receivedMessage}.");
        }

        protected virtual void OnResponseWritten(BasicDeliverEventArgs ea, string response)
        {
            _logger.LogInformation($"Responded on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}, ReplyMessage: {response}");
        }

        protected virtual void OnMessageReceivedException(BasicDeliverEventArgs ea, Exception ex)
        {
            var errors = string.Join(Environment.NewLine, ex.ExtractMessages());
            _logger.LogError($"Failed on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}. Errors: {errors}");
        }

        protected virtual void OnMessageReceivedEnds(BasicDeliverEventArgs ea, IModel channel, string response, IBasicProperties receivedProperties, IBasicProperties replyProperties)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: "", routingKey: receivedProperties.ReplyTo, basicProperties: replyProperties, body: responseBytes);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}
