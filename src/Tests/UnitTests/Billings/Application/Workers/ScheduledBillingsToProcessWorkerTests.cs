using Billings.Application.Abstractions;
using Billings.Application.Workers;
using Billings.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Billings.Helpers;
using Xunit;

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
            var logger = new Mock<ILogger<ScheduledBillingsToProcessWorker>>();

            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repositoryMock.Object, logger.Object);

            // act
            var (consumer, channel) = sut.BuildConsumerAndChanel(expectedQueuename, connectionFactoryMock.Object);

            //assert
            consumer.Should().NotBeNull()
                .And.BeOfType<EventingBasicConsumer>()
                .And.BeAssignableTo<IBasicConsumer>();
            channel.Should().NotBeNull().And.Be(channelMock.Object);
        }

        [Fact]
        public async Task HandleProcessedBatchMessage_Should_GenerateDeserializableMathingBillingsAsync()
        {
            // arrange
            MockBuildConsumerDependencies(out _, out Mock<IConnectionFactory> connectionFactoryMock);

            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedBillings));
            var repository = BillingRepositoryMockBuilder.Create()
                .UpdateProcessedBatch(expectedBillings, Task.CompletedTask).Build();
            var logger = new Mock<ILogger<ScheduledBillingsToProcessWorker>>();
            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repository, logger.Object);

            // act
            var result = await sut.HandleProcessedBatchMessage(messageBytes);
            var serialized = JsonConvert.DeserializeObject<Billing[]>(result);

            //assert
            result.Should().NotBeNullOrEmpty();
            serialized.Should().NotBeNull().And.BeEquivalentTo(expectedBillings);
        }

        [Fact]
        public async Task WriteCustomersMessage_Should_GenerateDeserializableMathingBillingsAsync()
        {
            // arrange
            MockBuildConsumerDependencies(out _, out Mock<IConnectionFactory> connectionFactoryMock);

            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var repository = BillingRepositoryMockBuilder.Create()
                .GetPending(expectedBillings).Build();
            var logger = new Mock<ILogger<ScheduledBillingsToProcessWorker>>();
            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repository, logger.Object);

            // act
            var result = await sut.WriteResponseMessage();
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
