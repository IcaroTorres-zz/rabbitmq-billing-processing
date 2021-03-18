using Issuance.Api.Application.Abstractions;
using Issuance.Api.Domain.Models;
using Library.Results;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Issuance.Api.Workers
{
    public class ScheduledBillingsToProcessWorker : BackgroundService
    {
        private readonly IBillingRepository _repository;
        private readonly IConnectionFactory _factory;

        public ScheduledBillingsToProcessWorker(IBillingRepository repository, IConnectionFactory factory)
        {
            _repository = repository;
            _factory = factory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = BuildConsumer(channel, nameof(Billing));

            consumer.Received += async (model, ea) =>
            {
                Console.WriteLine(
                    $"Received on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                        $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}.");

                string response = null;
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    var processedBatch = JsonConvert.DeserializeObject<List<Billing>>(message);
                    await _repository.UpdateProcessedBatchAsync(processedBatch);

                    Console.WriteLine(
                        $"Processed on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                            $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}, Body: {message}.");

                    var pendingProcessing = await _repository.GetPendingAsync(default);
                    response = JsonConvert.SerializeObject(pendingProcessing);

                    Console.WriteLine(
                        $"Responded on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, " +
                            $"DeliveryTag: {ea.DeliveryTag}, Reply: {response}.");
                }
                catch (Exception ex)
                {
                    response = string.Join(Environment.NewLine, ex.ExtractMessages());
                    Console.WriteLine(
                        $"Failed on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, " +
                            $"DeliveryTag: {ea.DeliveryTag}. Errors: {response}");
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                        basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            return Task.CompletedTask;
        }

        private static EventingBasicConsumer BuildConsumer(IModel channel, string queueName)
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
    }
}
