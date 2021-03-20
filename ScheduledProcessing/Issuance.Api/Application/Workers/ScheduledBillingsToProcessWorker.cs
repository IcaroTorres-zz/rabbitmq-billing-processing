using Issuance.Api.Application.Abstractions;
using Issuance.Api.Domain.Models;
using Library.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Application.Workers
{
    public class ScheduledBillingsToProcessWorker : BackgroundService
    {
        private readonly IConnectionFactory _factory;
        private readonly IBillingRepository _repository;
        private readonly ILogger<ScheduledBillingsToProcessWorker> _logger;
        private IModel _channel;

        public ScheduledBillingsToProcessWorker(
            IConnectionFactory factory,
            IBillingRepository repository,
            ILogger<ScheduledBillingsToProcessWorker> logger)
        {
            _repository = repository;
            _factory = factory;
            _logger = logger;
        }

        [ExcludeFromCodeCoverage]
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = BuildConsumer(nameof(Billing));
            consumer.Received += OnMessageReceived;
            return Task.CompletedTask;
        }

        internal EventingBasicConsumer BuildConsumer(string queueName)
        {
            var connection = _factory.CreateConnection();
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

        internal async Task<string> HandleProcessedBatchMessage(byte[] body)
        {
            var receivedMessage = Encoding.UTF8.GetString(body);
            var processedBatch = JsonConvert.DeserializeObject<List<Billing>>(receivedMessage);
            await _repository.UpdateProcessedBatchAsync(processedBatch);
            return receivedMessage;
        }

        internal async Task<string> WritePendingBillingsMessage()
        {
            var pendingProcessing = await _repository.GetPendingAsync(default);
            return JsonConvert.SerializeObject(pendingProcessing);
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
            _logger.LogInformation(
                $"Received on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                    $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}.");
            
            string response = null;
            var body = ea.Body.ToArray();
            var replyProperties = _channel.CreateBasicProperties();
            replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;

            try
            {
                string receivedMessage = await HandleProcessedBatchMessage(body);
                _logger.LogInformation(
                    $"Processed on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                        $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}, Body: {receivedMessage}.");

                response = await WritePendingBillingsMessage();
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
