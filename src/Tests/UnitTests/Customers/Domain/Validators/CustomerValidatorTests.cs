using Customers.Domain.Validators;
using FluentAssertions;
using Library.TestHelpers;
using UnitTests.Customers.Helpers;
using Xunit;

namespace UnitTests.Customers.Application.Validators
{
    [Trait("customers", "domain")]
    public class CustomerValidatorTests
    {
        [Fact]
        public void Should_Succeed()
        {
            // arrange
            var customer = InternalFakes.Customers.Valid().Generate();
            var sut = new CustomerValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_By_EmptyName()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var customer = InternalFakes.Customers.EmptyName().Generate();
            var sut = new CustomerValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_EmptyState()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var customer = InternalFakes.Customers.EmptyState().Generate();
            var sut = new CustomerValidator();

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
            var customer = InternalFakes.Customers.InvalidCpf().Generate();
            var sut = new CustomerValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidState()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var customer = InternalFakes.Customers.InvalidState().Generate();
            var sut = new CustomerValidator();

            // act
            var result = sut.Validate(customer);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
