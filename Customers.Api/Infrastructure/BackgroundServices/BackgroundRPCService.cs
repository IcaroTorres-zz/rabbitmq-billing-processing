using Customers.Api.Application.Abstractions;
using Customers.Api.Domain.Models;
using Customers.Api.Infrastructure.Persistence;
using Library.DependencyInjection;
using Library.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Infrastructure.BackgroundServices
{
    public class BackgroundRPCService : BackgroundService
    {
        private readonly string amqpUrl;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        public BackgroundRPCService(RabbitMQSettings settings, IWebHostEnvironment env, IConfiguration configuration)
        {
            amqpUrl = settings.AmqpUrl;
            this.env = env;
            this.configuration = configuration;
        }

        private ICustomerRepository BuildRepository()
        {
            var sourcePath = Path.Combine(env.ContentRootPath, ".\\Infrastructure\\Persistence", configuration["SQLite:DatabaseName"]);
            var builder = new DbContextOptionsBuilder<CustomersContext>();
            builder.UseSqlite($"Data Source={sourcePath}");
            if (!env.IsProduction()) builder.EnableSensitiveDataLogging();
            var context = new CustomersContext(builder.Options);
            return new CustomerRepository(context);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var repository = BuildRepository();
            var factory = new ConnectionFactory { Uri = new Uri(amqpUrl) };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = InitializerConsumer(channel, nameof(Customer));

            consumer.Received += async (model, ea) =>
            {
                var responseMessage = "";
                try
                {
                    var customers = await repository.GetAllAsync();
                    responseMessage = JsonConvert.SerializeObject(customers);
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
