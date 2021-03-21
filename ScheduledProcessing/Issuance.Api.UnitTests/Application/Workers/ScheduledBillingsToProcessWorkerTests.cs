using FluentAssertions;
using Issuance.Api.Application.Abstractions;
using Issuance.Api.Application.Workers;
using Issuance.Api.Domain.Models;
using Issuance.Api.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Issuance.Api.UnitTests.Application.Workers
{
    [Trait("unit-test", "issuance-application")]
    public class ScheduledBillingsToProcessWorkerTests
    {
        [Fact]
        public void BuildConsumer_Should_Create_EventingBasicConsumer()
        {
            // arrange
            const string expectedQueuename = "sample";
            MockBuildConsumerDependencies(
                expectedQueuename,
                out Mock<IModel> channelMock,
                out Expression<Action<IModel>> queueDeclareExpression,
                out Expression<Action<IModel>> basicQosExpression,
                out Mock<IConnectionFactory> connectionFactoryMock);

            var repositoryMock = new Mock<IBillingRepository>();
            var logger = new Mock<ILogger<ScheduledBillingsToProcessWorker>>();

            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repositoryMock.Object, logger.Object);

            // act
            var result = sut.BuildConsumer(expectedQueuename);

            //assert
            result.Should().NotBeNull()
                .And.BeOfType<EventingBasicConsumer>()
                .And.BeAssignableTo<IBasicConsumer>();
            result.Model.Should().NotBeNull().And.Be(channelMock.Object);
            channelMock.Verify(queueDeclareExpression, Times.Once());
            channelMock.Verify(basicQosExpression, Times.Once());
        }

        [Fact]
        public async Task HandleProcessedBatchMessage_Should_GenerateDeserializableMathingBillingsAsync()
        {
            // arrange
            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedBillings));
            var repository = BillingRepositoryMockBuilder.Create()
                .UpdateProcessedBatch(expectedBillings, Task.CompletedTask).Build();
            var connectionFactoryMock = new Mock<IConnectionFactory>();
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
            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var repository = BillingRepositoryMockBuilder.Create()
                .GetPending(expectedBillings).Build();
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            var logger = new Mock<ILogger<ScheduledBillingsToProcessWorker>>();
            var sut = new ScheduledBillingsToProcessWorker(
                connectionFactoryMock.Object, repository, logger.Object);

            // act
            var result = await sut.WritePendingBillingsMessage();
            var serialized = JsonConvert.DeserializeObject<Billing[]>(result);

            //assert
            result.Should().NotBeNullOrEmpty();
            serialized.Should().NotBeNull().And.BeEquivalentTo(expectedBillings);
        }

        private static void MockBuildConsumerDependencies(string expectedQueuename, out Mock<IModel> channelMock, out Expression<Action<IModel>> queueDeclareExpression, out Expression<Action<IModel>> basicQosExpression, out Mock<IConnectionFactory> connectionFactoryMock)
        {
            channelMock = new Mock<IModel>();
            queueDeclareExpression = x => x.QueueDeclare(
                expectedQueuename,
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object>>());
            channelMock.Setup(queueDeclareExpression).Verifiable();
            basicQosExpression = x => x.BasicQos(0, 1, false);
            channelMock.Setup(basicQosExpression).Verifiable();

            var connectionMock = new Mock<IConnection>();
            connectionMock.Setup(x => x.CreateModel()).Returns(channelMock.Object);
            connectionFactoryMock = new Mock<IConnectionFactory>();
            connectionFactoryMock.Setup(x => x.CreateConnection()).Returns(connectionMock.Object);
        }
    }
}
