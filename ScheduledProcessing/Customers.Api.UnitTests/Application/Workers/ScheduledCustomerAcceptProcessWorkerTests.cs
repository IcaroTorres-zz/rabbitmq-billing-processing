using Customers.Api.Application.Workers;
using Customers.Api.Domain.Models;
using Customers.Api.Infrastructure.Persistence;
using Customers.Api.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Customers.Api.UnitTests.Application.Workers
{
    [Trait("unit-test", "customers-application")]
    public class ScheduledCustomerAcceptProcessWorkerTests
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

            var repositoryFactoryMock = new Mock<ICustomerRepositoryFactory>();
            var logger = new Mock<ILogger<ScheduledCustomerAcceptProcessWorker>>();

            var sut = new ScheduledCustomerAcceptProcessWorker(
                connectionFactoryMock.Object, repositoryFactoryMock.Object, logger.Object);

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
        public async Task WriteCustomersMessage_Should_GenerateDeserializableMathingCustomersAsync()
        {
            // arrange
            var expectedCustomers = InternalFakes.Customers.Valid().Generate(2);
            var repository = CustomerRepositoryMockBuilder.Create()
                .GetAll(expectedCustomers).Build();
            var repositoryFactoryMock = new Mock<ICustomerRepositoryFactory>();
            repositoryFactoryMock.Setup(x => x.CreateRepository()).Returns(repository);

            var connectionFactoryMock = new Mock<IConnectionFactory>();
            var logger = new Mock<ILogger<ScheduledCustomerAcceptProcessWorker>>();
            var sut = new ScheduledCustomerAcceptProcessWorker(
                connectionFactoryMock.Object, repositoryFactoryMock.Object, logger.Object);

            // act
            var result = await sut.WriteCustomersMessage();
            var serialized = JsonConvert.DeserializeObject<Customer[]>(result);

            //assert
            result.Should().NotBeNullOrEmpty();
            serialized.Should().NotBeNull().And.BeEquivalentTo(expectedCustomers);
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
