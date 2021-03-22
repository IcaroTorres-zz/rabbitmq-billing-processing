using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PrivatePackage.Abstractions;
using PrivatePackage.Results;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PrivatePackage.Messaging
{
    public class ConsumerRabbitMQ<T> : IMessageConsumer<T> where T : IRequest<IResult>
    {
        private readonly IConnection connection;
        private readonly IRabbitMQSettings settings;
        private readonly IMediator mediator;
        private readonly ILogger<ConsumerRabbitMQ<T>> logger;

        public ConsumerRabbitMQ(
            IConnection connection,
            IRabbitMQSettings settings,
            IMediator mediator,
            ILogger<ConsumerRabbitMQ<T>> logger)
        {
            this.connection = connection;
            this.mediator = mediator;
            this.logger = logger;
            this.settings = settings;
        }

        public async Task ConsumeWithUsecase(string consumerName)
        {
            await Task.Run(() =>
            {
                var channel = connection.CreateModel();
                var exchangeSettings = settings.ConsumeExchanges.GetSettings(consumerName);

                channel.ExchangeDeclare(exchange: exchangeSettings.Name, type: exchangeSettings.Type);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: exchangeSettings.Name, routingKey: exchangeSettings.RoutingKey);

                if (settings.DispatchConsumersAsync)
                {
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) => await OnMessageReceived(channel, consumerName, queueName, ea);
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                }
                else
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) => await OnMessageReceived(channel, consumerName, queueName, ea);
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                }
            });
        }

        private async Task OnMessageReceived(IModel channel, string consumerName, string queueName, BasicDeliverEventArgs ea)
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var usecaseResult = await SendMessageToUsecase(body);
            if (usecaseResult.IsSuccess())
            {
                logger.LogInformation($"Message consumed. RoutingKey: {ea.RoutingKey}, Consumer: {consumerName}, " +
                    $"Queue: {queueName}, DeliveryTag: {ea.DeliveryTag}, Body: {body}.");
                channel.BasicAck(ea.DeliveryTag, false);
            }
            else
            {
                var errorLines = string.Join(Environment.NewLine, usecaseResult.Errors);
                logger.LogError($"Message consumption failed. RoutingKey: {ea.RoutingKey}, Consumer: {consumerName}, " +
                    $"Queue: {queueName}, DeliveryTag: {ea.DeliveryTag}, Body: {body}. Errors: {errorLines}");
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        private async Task<IResult> SendMessageToUsecase(string stringMessage)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<T>(stringMessage, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });
                return await mediator.Send(message);
            }
            catch (Exception ex)
            {
                return new FailResult(StatusCodes.Status500InternalServerError, ex.ExtractMessages());
            }
        }
    }
}
