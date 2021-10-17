using Billings.Domain.Validators;
using FluentAssertions;
using Library.TestHelpers;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Application.Validators
{
    [Trait("billings", "domain")]
    public class BillingValidatorTests
    {
        [Fact]
        public void Should_Succeed()
        {
            // arrange
            var customer = InternalFakes.Billings.Valid().Generate();
            var sut = new BillingValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_By_InvalidAmount()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var customer = InternalFakes.Billings.InvalidAmount().Generate();
            var sut = new BillingValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidDate()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var customer = InternalFakes.Billings.InvalidDate().Generate();
            var sut = new BillingValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidCpf()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var customer = InternalFakes.Billings.InvalidCpf().Generate();
            var sut = new BillingValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
