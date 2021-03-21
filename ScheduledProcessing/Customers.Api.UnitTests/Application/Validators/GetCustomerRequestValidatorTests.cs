using Customers.Api.Application.Validators;
using Customers.Api.UnitTests.Helpers;
using FluentAssertions;
using Library.TestHelpers;
using Xunit;

namespace Customers.Api.UnitTests.Application.Validators
{
    [Trait("unit-test", "customers.api-application")]
    public class GetCustomerRequestValidatorTests
    {
        [Fact]
        public void Should_Succeed()
        {
            // arrange
            var request = InternalFakes.GetCustomerRequests.Valid().Generate();
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, true).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new GetCustomerRequestValidator(cpfValidator, repository);

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
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, true).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateFalse().Build();

            var sut = new GetCustomerRequestValidator(cpfValidator, repository);

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
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, false).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new GetCustomerRequestValidator(cpfValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
