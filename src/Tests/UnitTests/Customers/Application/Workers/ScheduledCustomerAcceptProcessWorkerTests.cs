using Customers.Application.Abstractions;
using Customers.Application.Workers;
using Customers.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using UnitTests.Customers.Helpers;
using Xunit;
using static Library.TestHelpers.Fakes;

namespace UnitTests.Customers.Application.Workers
{
    [Trait("customers", "application")]
    public class ScheduledCustomerAcceptProcessWorkerTests
    {
        [Fact]
        public void BuildConsumer_Should_Create_EventingBasicConsumer()
        {
            // arrange
            const string expectedQueuename = "sample";
            MockBuildConsumerDependencies(out var channelMock, out var connectionFactoryMock);

            var repositoryFactoryMock = new Mock<ICustomerRepositoryFactory>();
            var logger = new Mock<ILogger>();

            var sut = new ScheduledCustomerAcceptProcessWorker(
                connectionFactoryMock.Object, repositoryFactoryMock.Object, logger.Object);

            // act

            var consumer = sut.BuildConsumer(expectedQueuename, connectionFactoryMock.Object);

            //assert
            consumer.Should().NotBeNull()
                .And.BeOfType<EventingBasicConsumer>()
                .And.BeAssignableTo<IBasicConsumer>();
            consumer.Model.Should().NotBeNull().And.Be(channelMock.Object);
        }

        [Fact]
        public async Task HandleReceivedMessage_Should_GenerateDeserializableMathingBillingsAsync()
        {
            // arrange
            MockBuildConsumerDependencies(out _, out var connectionFactoryMock);
            var expectedMessage = string.Empty;
            var deliverEventArgs = DeliverEventArgs.WithBody(expectedMessage).Generate();
            var repository = CustomerRepositoryMockBuilder.Create().Build();
            var repositoryFactoryMock = new Mock<ICustomerRepositoryFactory>();
            repositoryFactoryMock.Setup(x => x.CreateRepository()).Returns(repository);
            var logger = new Mock<ILogger>();
            var sut = new ScheduledCustomerAcceptProcessWorker(
                connectionFactoryMock.Object, repositoryFactoryMock.Object, logger.Object);

            // act
            var (receivedValue, receivedMessage) = await sut.HandleReceivedMessage(deliverEventArgs);

            //assert
            receivedValue.Should().BeNull();
            receivedMessage.Should().BeNullOrEmpty().And.Be(expectedMessage);
        }


        [Fact]
        public async Task WriteResponseMessage_Should_GenerateDeserializableMathingCustomersAsync()
        {
            // arrange
            MockBuildConsumerDependencies(out _, out var connectionFactoryMock);
            var expectedCustomers = InternalFakes.Customers.Valid().Generate(2);
            var repository = CustomerRepositoryMockBuilder.Create()
                .GetAll(expectedCustomers).Build();
            var repositoryFactoryMock = new Mock<ICustomerRepositoryFactory>();
            repositoryFactoryMock.Setup(x => x.CreateRepository()).Returns(repository);

            var logger = new Mock<ILogger>();
            var sut = new ScheduledCustomerAcceptProcessWorker(
                connectionFactoryMock.Object, repositoryFactoryMock.Object, logger.Object);

            // act
            var result = await sut.WriteResponseMessage(default);
            var serialized = JsonConvert.DeserializeObject<Customer[]>(result);

            //assert
            result.Should().NotBeNullOrEmpty();
            serialized.Should().NotBeNull().And.BeEquivalentTo(expectedCustomers);
        }

        private static void MockBuildConsumerDependencies(out Mock<IModel> channelMock, out Mock<IConnectionFactory> connectionFactoryMock)
        {
            channelMock = new Mock<IModel>();
            var connectionMock = new Mock<IConnection>();
            connectionMock.Setup(x => x.CreateModel()).Returns(channelMock.Object);
            connectionFactoryMock = new Mock<IConnectionFactory>();
            connectionFactoryMock.Setup(x => x.CreateConnection()).Returns(connectionMock.Object);
        }
    }
}
