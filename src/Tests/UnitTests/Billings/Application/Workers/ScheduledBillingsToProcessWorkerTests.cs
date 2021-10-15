using Billings.Application.Abstractions;
using Billings.Application.Workers;
using Billings.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using UnitTests.Billings.Helpers;
using Xunit;
using static Library.TestHelpers.Fakes;

namespace UnitTests.Billings.Application.Workers
{
    [Trait("billings", "application")]
    public class ScheduledBillingsToProcessWorkerTests
    {
        [Fact]
        public void BuildConsumer_Should_Create_EventingBasicConsumer()
        {
            // arrange
            const string expectedQueuename = "sample";
            MockBuildConsumerDependencies(out Mock<IModel> channelMock, out Mock<IConnectionFactory> connectionFactoryMock);

            var repositoryMock = new Mock<IBillingRepository>();
            var logger = new Mock<ILogger>();

            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repositoryMock.Object, logger.Object);

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
            MockBuildConsumerDependencies(out _, out Mock<IConnectionFactory> connectionFactoryMock);
            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var deliverEventArgs = DeliverEventArgs.WithBody(expectedBillings).Generate();
            var repository = BillingRepositoryMockBuilder.Create()
                .UpdateProcessedBatch(expectedBillings, Task.CompletedTask).Build();
            var logger = new Mock<ILogger>();
            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repository, logger.Object);

            // act
            var (receivedValue, receivedMessage) = await sut.HandleReceivedMessage(deliverEventArgs);

            //assert
            receivedMessage.Should().NotBeNullOrEmpty();
            receivedValue.Should().NotBeNull().And.BeEquivalentTo(expectedBillings);
        }

        [Fact]
        public async Task WriteResponseMessage_Should_GenerateDeserializableMathingBillingsAsync()
        {
            // arrange
            MockBuildConsumerDependencies(out _, out Mock<IConnectionFactory> connectionFactoryMock);

            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var repository = BillingRepositoryMockBuilder.Create()
                .GetPending(expectedBillings).Build();
            var logger = new Mock<ILogger>();
            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repository, logger.Object);

            // act
            var result = await sut.WriteResponseMessage(default);
            var serialized = JsonConvert.DeserializeObject<Billing[]>(result);

            //assert
            result.Should().NotBeNullOrEmpty();
            serialized.Should().NotBeNull().And.BeEquivalentTo(expectedBillings);
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
