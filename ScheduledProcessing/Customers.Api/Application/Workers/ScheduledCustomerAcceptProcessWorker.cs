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

        public ScheduledCustomerAcceptProcessWorker(
            IConnectionFactory connectionFactory,
            ICustomerRepositoryFactory repositoryFactory,
            ILogger<ScheduledCustomerAcceptProcessWorker> logger)
        {
            _connectionFactory = connectionFactory;
            _repositoryFactory = repositoryFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = DoExecute(nameof(Customer));

            return Task.CompletedTask;
        }

        internal EventingBasicConsumer DoExecute(string queueName)
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = BuildConsumer(channel, queueName);

            consumer.Received += async (model, ea) =>
            {
                string response = null;
                var replyProperties = channel.CreateBasicProperties();
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
                    FinalizeReceivedMessage(ea, channel, response, ea.BasicProperties, replyProperties);
                }
            };

            return consumer;
        }

        internal EventingBasicConsumer BuildConsumer(IModel channel, string queueName)
        {
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
    }
}
