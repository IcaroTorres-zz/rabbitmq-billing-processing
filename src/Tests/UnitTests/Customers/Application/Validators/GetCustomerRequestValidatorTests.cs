using Customers.Application.Validators;
using FluentAssertions;
using Library.TestHelpers;
using UnitTests.Customers.Helpers;
using Xunit;

namespace UnitTests.Customers.Application.Validators
{
    [Trait("customers", "application")]
    public class GetCustomerRequestValidatorTests
    {
        [Fact]
        public void Should_Succeed()
        {
            // arrange
            var request = InternalFakes.GetCustomerRequests.Valid().Generate();
            var repository = CustomerRepositoryMockBuilder.Create().Exists(request.Cpf, true).Build();
            var sut = new GetCustomerRequestValidator(repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_By_InvalidCpf()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.GetCustomerRequests.InvalidCpf().Generate();
            var repository = CustomerRepositoryMockBuilder.Create().Exists(request.Cpf, true).Build();
            var sut = new GetCustomerRequestValidator(repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_CustomerNotFound()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.GetCustomerRequests.Valid().Generate();
            var repository = CustomerRepositoryMockBuilder.Create().Exists(request.Cpf, false).Build();
            var sut = new GetCustomerRequestValidator(repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
