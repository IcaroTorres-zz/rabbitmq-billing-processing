using Customers.Api.Domain.Models;
using Customers.Api.Infrastructure.Persistence;
using Library.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Workers
{
    public class ScheduledCustomerAcceptProcessWorker : BackgroundService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ICustomerRepositoryFactory _repositoryFactory;
        private readonly ILogger<ScheduledCustomerAcceptProcessWorker> _logger;
        private IModel _channel;

        public ScheduledCustomerAcceptProcessWorker(
            IConnectionFactory connectionFactory,
            ICustomerRepositoryFactory repositoryFactory,
            ILogger<ScheduledCustomerAcceptProcessWorker> logger)
        {
            _connectionFactory = connectionFactory;
            _repositoryFactory = repositoryFactory;
            _logger = logger;
        }

        [ExcludeFromCodeCoverage]
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = BuildConsumer(nameof(Customer));
            consumer.Received += OnMessageReceived;
            return Task.CompletedTask;
        }

        internal EventingBasicConsumer BuildConsumer(string queueName)
        {
            var connection = _connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine($"Awaiting RPC requests for queue {queueName}");
            return consumer;
        }

        internal async Task<string> WriteCustomersMessage()
        {
            var repository = _repositoryFactory.CreateRepository();
            var customers = await repository.GetAllAsync();
            return JsonConvert.SerializeObject(customers);
        }

        [ExcludeFromCodeCoverage]
        private void FinalizeReceivedMessage(
            BasicDeliverEventArgs ea,
            IModel channel,
            string response,
            IBasicProperties receivedProperties,
            IBasicProperties replyProperties)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: "", routingKey: receivedProperties.ReplyTo,
                basicProperties: replyProperties, body: responseBytes);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        private async void OnMessageReceived(object model, BasicDeliverEventArgs ea)
        {
            string response = null;
            var replyProperties = _channel.CreateBasicProperties();
            replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;

            _logger.LogInformation(
                $"Received on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                    $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}.");
            try
            {
                response = await WriteCustomersMessage();
                _logger.LogInformation(
                    $"Responded on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                        $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}, ReplyMessage: {response}");
            }
            catch (Exception ex)
            {
                var errors = string.Join(Environment.NewLine, ex.ExtractMessages());
                _logger.LogError(
                    $"Failed on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                        $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}. Errors: {errors}");
            }
            finally
            {
                FinalizeReceivedMessage(ea, _channel, response, ea.BasicProperties, replyProperties);
            }
        }
    }
}
