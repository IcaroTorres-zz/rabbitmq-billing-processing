using Customers.Api.Application.Responses;
using Customers.Api.Application.Usecases;
using FluentAssertions;
using Library.Results;
using Microsoft.AspNetCore.Http;
using ScheduledProcessing.Tests.Customers.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace ScheduledProcessing.Tests.Customers.UnitTests.Application.Usecases
{
    [Trait("unit-test", "customers-application")]
    public class GetCustomerUsecaseTests
    {
        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommand()
        {
            // arrange
            var expectedCustomer = InternalFakes.Customers.Valid().Generate();
            var expectedCpf = expectedCustomer.Cpf.ToString("00000000000");
            var request = InternalFakes.GetCustomerRequests.Valid().Generate();
            var repository = CustomerRepositoryMockBuilder.Create()
                .Get(request.Cpf, expectedCustomer).Build();
            var sut = new GetCustomerUsecase(repository);

            // act
            var result = await sut.Handle(request, default);
            var resultData = result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(StatusCodes.Status200OK);
            resultData.Should().NotBeNull().And.BeOfType<CustomerResponse>();
            resultData.As<CustomerResponse>().Cpf.Should().Be(expectedCpf);
            resultData.As<CustomerResponse>().Name.Should().Be(expectedCustomer.Name);
            resultData.As<CustomerResponse>().State.Should().Be(expectedCustomer.State);
        }
    }
}
