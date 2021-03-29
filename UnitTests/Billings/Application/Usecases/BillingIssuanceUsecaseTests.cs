using Billings.Application.Models;
using Billings.Application.Usecases;
using FluentAssertions;
using Library.Messaging;
using Library.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Application.Usecases
{
    [Trait("billings", "application")]
    public class BillingIssuanceUsecaseTests
    {
        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommand()
        {
            // arrange
            var request = InternalFakes.BillingRequests.Valid().Generate();
            const int expectedStatus = StatusCodes.Status201Created;
            var expectedBilling = InternalFakes.Billings.Valid().Generate();
            var factory = ModelFactoryMockBuilder.Create().CreateBilling(
                cpfString: request.Cpf,
                amount: request.Amount,
                dueDate: request.DueDate,
                model: expectedBilling).Build();

            var repository = BillingRepositoryMockBuilder.Create()
                .Insert(expectedBilling, Task.CompletedTask).Build();

            var publisherMock = new Mock<IMessagePublisher>();
            publisherMock.Setup(x => x.Publish(It.IsAny<MessageBase>())).Returns(Task.CompletedTask);
            var sut = new BillingIssuanceUsecase(factory, repository, publisherMock.Object);

            // act
            var result = await sut.Handle(request, default);
            var resultData = (BillingResponse)result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(expectedStatus);
            resultData.Should().NotBeNull().And.BeOfType<BillingResponse>();
            resultData.Id.Should().Be(expectedBilling.Id);
            resultData.Cpf.Should().Be(expectedBilling.Cpf.ToString("00000000000"));
            resultData.Amount.Should().Be(expectedBilling.Amount);
            resultData.DueDate.Should().Be(expectedBilling.DueDate.ToString());
        }
    }
}
