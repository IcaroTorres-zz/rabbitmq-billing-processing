using Newtonsoft.Json;
using Processing.Worker.Domain.Models;
using Processing.Worker.Domain.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Processing.Worker
{
    public static class Program
    {
        private static readonly IAmountProcessor processor = new MathOnlyAmountProcessor();

        private static readonly ConcurrentDictionary<ulong, Customer> customersMap = new ConcurrentDictionary<ulong, Customer>();

        private static readonly ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, Billing>> unprocessedMap =
            new ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, Billing>>();

        private static readonly ConcurrentDictionary<Guid, Billing> processedMap = new ConcurrentDictionary<Guid, Billing>();

        private static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://xusddevf:aEGQBjRmboL4oP6Tx3kqSu2sn7nZ2j-8@jackal.rmq.cloudamqp.com/xusddevf")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var (customersQueue, customersProperties) = SetupChannelForRPC(channel, nameof(Customer), OnCustomersResponse);
            var (billingsQueue, billintsProperties) = SetupChannelForRPC(channel, nameof(Billing), OnBillingsResponse);

            while (true)
            {
                var processedBatchBody = PrepareProcessedBatchMessage();

                channel.BasicPublish(
                    exchange: "", routingKey: billingsQueue, basicProperties: billintsProperties, body: processedBatchBody);

                channel.BasicPublish(
                    exchange: "", routingKey: customersQueue, basicProperties: customersProperties, body: default);

                ResetStructures();
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private static void ResetStructures()
        {
            unprocessedMap.Clear();
            processedMap.Clear();
            customersMap.Clear();
        }

        private static void OnCustomersResponse(string message)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(message);
            Parallel.ForEach(customers, customer =>
            {
                customersMap.TryAdd(customer.Cpf, customer);
                unprocessedMap.TryAdd(customer.Cpf, new ConcurrentDictionary<Guid, Billing>());

                if (unprocessedMap.TryGetValue(customer.Cpf, out var billings))
                {
                    Parallel.ForEach(billings, pair =>
                    {
                        var (id, billing) = pair;
                        billing = processor.Process(customer, billing);
                        processedMap.TryAdd(id, billing);
                        billings.TryRemove(id, out _);
                    });
                }
            });
        }

        private static void OnBillingsResponse(string message)
        {
            var billings = JsonConvert.DeserializeObject<List<Billing>>(message);
            Parallel.ForEach(billings, billing =>
            {
                if (customersMap.TryGetValue(billing.Cpf, out var customer))
                {
                    billing = processor.Process(customer, billing);
                    processedMap.TryAdd(billing.Id, billing);
                }
                else
                {
                    unprocessedMap.TryAdd(billing.Cpf, new ConcurrentDictionary<Guid, Billing>());
                    unprocessedMap.TryGetValue(billing.Cpf, out var awaitingBillings);
                    awaitingBillings.TryAdd(billing.Id, billing);
                }
            });
        }

        private static byte[] PrepareProcessedBatchMessage()
        {
            var processed = processedMap.Values;
            var message = JsonConvert.SerializeObject(processed);
            var body = Encoding.UTF8.GetBytes(message);
            return body;
        }

        private static (string requestQueue, IBasicProperties properties) SetupChannelForRPC(IModel channel, string requestQueue, Action<string> successCallback)
        {
            var correlationId = Guid.NewGuid().ToString();
            var responseQueue = $"{requestQueue}_response";
            channel.QueueDeclare(
                requestQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueDeclare(
                responseQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId != correlationId)
                {
                    Console.WriteLine($"Coreção incongruente entre {correlationId} e recebido {ea.BasicProperties.CorrelationId}");
                    return;
                }

                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Received message from queue: {responseQueue}, body: {message}");
                successCallback(message);
            };
            channel.BasicConsume(queue: responseQueue, autoAck: true, consumer: consumer);

            var properties = channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = responseQueue;
            return (requestQueue, properties);
        }
    }
}
