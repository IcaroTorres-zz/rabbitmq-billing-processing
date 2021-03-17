using Customers.Api.Application.Abstractions;
using Customers.Api.Domain.Models;
using Customers.Api.Infrastructure.Persistence;
using Library.DependencyInjection;
using Library.Results;
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

namespace Customers.Api.Workers
{
    public class ScheduledCustomerAcceptProcessWorker : BackgroundService
    {
        private readonly string amqpUrl;
        private readonly IHostEnvironment env;
        private readonly IConfiguration configuration;

        public ScheduledCustomerAcceptProcessWorker(RabbitMQSettings settings, IHostEnvironment env, IConfiguration configuration)
        {
            amqpUrl = settings.AmqpUrl;
            this.env = env;
            this.configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { Uri = new Uri(amqpUrl) };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = BuildConsumer(channel, nameof(Customer));

            consumer.Received += async (model, ea) =>
            {
                string response = null;
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(
                    $"Received {message} on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                        $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}.");

                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    Console.WriteLine(
                        $"Processed {message} on CorrelationId: {ea.BasicProperties.CorrelationId}, " +
                            $"RoutingKey: {ea.RoutingKey}, DeliveryTag: {ea.DeliveryTag}, Body: {message}.");

                    var repository = BuildRepository();
                    var customers = await repository.GetAllAsync();
                    response = JsonConvert.SerializeObject(customers);

                    Console.WriteLine(
                        $"Responded {message} on CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, " +
                            $"DeliveryTag: {ea.DeliveryTag}, Reply: {response}.");
                }
                catch (Exception ex)
                {
                    response = string.Join(Environment.NewLine, ex.ExtractMessages());
                    Console.WriteLine(
                        $"Failed on {message} CorrelationId: {ea.BasicProperties.CorrelationId}, RoutingKey: {ea.RoutingKey}, " +
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

        private ICustomerRepository BuildRepository()
        {
            var sourcePath = Path.Combine(env.ContentRootPath, "Infrastructure", "Persistence", configuration["SQLite:DatabaseName"]);
            var builder = new DbContextOptionsBuilder<CustomersContext>();
            builder.UseSqlite($"Data Source={sourcePath}");
            if (!env.IsProduction()) builder.EnableSensitiveDataLogging();
            var context = new CustomersContext(builder.Options);
            return new CustomerRepository(context);
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
