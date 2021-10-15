using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Library.Messaging
{
    public interface IRpcServer<T> : IHostedService, IDisposable
    {
        EventHandler<BasicDeliverEventArgs> OnMessageReceived { get; }
        EventingBasicConsumer BuildConsumer(string queueName, IConnectionFactory connectionFactory);
        IBasicProperties CreateBasicProperties(BasicDeliverEventArgs ea);
        Task<(T receivedValue, string receivedMessage)> HandleReceivedMessage(BasicDeliverEventArgs ea);
        Task<string> WriteResponseMessage(T receivedValue);
    }
}
