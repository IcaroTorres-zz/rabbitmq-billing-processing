using Billings.Application.Usecases;
using Billings.Domain.Models;
using FluentAssertions;
using Library.Messaging;
using Library.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Application.Usecases
{
    [Trait("billings", "application")]
    public class ConfirmProcessedBatchUsecaseTests
    {
        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommand()
        {
            // arrange
            var expectedCount = 5;
            var request = InternalFakes.ProcessedBatchs.Processed(expectedCount).Generate();
            var processedCount = request.Count(x => x.ProcessedAt != null);
            const int expectedStatus = StatusCodes.Status200OK;

            var repository = BillingRepositoryMockBuilder.Create()
                .UpdateProcessedBatch(request, Task.CompletedTask).Build();

            var publisherMock = new Mock<IMessagePublisher>();
            publisherMock.Setup(x => x.Publish(It.IsAny<MessageBase>())).Returns(Task.CompletedTask);
            var sut = new ConfirmProcessedBatchUsecase(repository, publisherMock.Object);

            // act
            var result = await sut.Handle(request, default);
            var resultData = (ProcessedBatch)result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(expectedStatus);
            resultData.Should().NotBeNull().And
                .BeOfType<ProcessedBatch>().And
                .HaveCount(expectedCount).And
                .BeEquivalentTo(request).And
                .Match(x => x.Count(y => y.ProcessedAt != null) == processedCount);
        }
    }
}
