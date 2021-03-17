using Issuance.Api.Application.Abstractions;
using Issuance.Api.Domain.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Library.Results;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Library.DependencyInjection;

namespace Issuance.Api.Infrastructure.BackgroundServices
{
    public class BackgroundRPCService : BackgroundService
    {
        private readonly IBillingRepository repository;
        private readonly string amqpUrl;

        public BackgroundRPCService(IBillingRepository repository, RabbitMQSettings settings)
        {
            this.repository = repository;
            amqpUrl = settings.AmqpUrl;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { Uri = new Uri(amqpUrl) };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = InitializerConsumer(channel, nameof(Billing));

            consumer.Received += async (model, ea) =>
            {
                var responseMessage = "";
                try
                {
                    var incommingMessage = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var processedBillings = JsonConvert.DeserializeObject<List<Billing>>(incommingMessage);
                    await repository.UpdateProcessedBatchAsync(processedBillings);

                    var pendingProcessing = await repository.GetPendingAsync(default);
                    responseMessage = JsonConvert.SerializeObject(pendingProcessing);
                    Console.WriteLine(
                        $"Received on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                            $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}.");

                    ReplyMessage(responseMessage, channel, ea);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    var errorLines = string.Join(Environment.NewLine, ex.ExtractMessages());
                    Console.WriteLine(
                        $"Failed on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, " +
                            $"DeliveryTag: {ea.DeliveryTag}. Body: {responseMessage}. Errors: {errorLines}");

                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };
            return Task.CompletedTask;
        }

        private static void ReplyMessage(string responseMessage, IModel channel, BasicDeliverEventArgs ea)
        {
            var requestProps = ea.BasicProperties;
            var responseProps = channel.CreateBasicProperties();
            responseProps.CorrelationId = requestProps.CorrelationId;

            var responseBytes = Encoding.UTF8.GetBytes(responseMessage);

            channel.BasicPublish(exchange: "", routingKey: requestProps.ReplyTo,
                basicProperties: responseProps, body: responseBytes);
            Console.WriteLine(
                $"Responded on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, " +
                    $"DeliveryTag: {ea.DeliveryTag}, Response: {responseMessage}.");
        }

        private static EventingBasicConsumer InitializerConsumer(IModel channel, string queueName)
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

            return consumer;
        }
    }
}
