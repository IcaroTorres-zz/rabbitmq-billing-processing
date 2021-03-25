using FluentAssertions;
using Issuance.Api.Application.Validators;
using Issuance.Api.UnitTests.Helpers;
using Library.TestHelpers;
using Xunit;

namespace Issuance.Api.UnitTests.Application.Validators
{
    [Trait("unit-test", "issuance.api-application")]
    public class BillingRequestValidatorTests
    {
        [Fact]
        public void Should_Succeed_WithValidRequest()
        {
            // arrange
            var request = InternalFakes.BillingRequests.Valid().Generate();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new BillingRequestValidator(cpfValidator);

            // act
            var result = sut.Validate(request);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_By_InvalidAmount()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.BillingRequests.InvalidAmount().Generate();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new BillingRequestValidator(cpfValidator);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidCpf()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.BillingRequests.InvalidCpf().Generate();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateFalse().Build();

            var sut = new BillingRequestValidator(cpfValidator);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidDueDate()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.BillingRequests.InvalidDueDate().Generate();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateFalse().Build();

            var sut = new BillingRequestValidator(cpfValidator);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
