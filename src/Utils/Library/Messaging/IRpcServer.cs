using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Library.Messaging
{
    public interface IRpcServer<T> : IHostedService, IDisposable where T : IRpcServer<T>
    {
        EventHandler<BasicDeliverEventArgs> OnMessageReceived { get; }
        (EventingBasicConsumer, IModel) BuildConsumerAndChanel(string queueName, IConnectionFactory connectionFactory);
        IBasicProperties CreateBasicProperties(BasicDeliverEventArgs ea);
        Task<string> HandleReceivedMessage(BasicDeliverEventArgs ea);
        Task<string> WriteResponseMessage();
    }
}
