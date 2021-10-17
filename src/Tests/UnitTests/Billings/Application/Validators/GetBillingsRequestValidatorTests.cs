using Billings.Application.Validators;
using FluentAssertions;
using Library.TestHelpers;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Application.Validators
{
    [Trait("billings", "application")]
    public class GetBillingsRequestValidatorTests
    {
        [Fact]
        public void Should_Succeed_WithValidCpf()
        {
            // arrange
            var request = InternalFakes.GetBillingsRequests.ValidWithCpf().Generate();
            var sut = new GetBillingsRequestValidator();

            // act
            var result = sut.Validate(request);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Succeed_WithValidMonth()
        {
            // arrange
            var request = InternalFakes.GetBillingsRequests.ValidWithMonth().Generate();
            var sut = new GetBillingsRequestValidator();

            // act
            var result = sut.Validate(request);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Succeed_WithValidCpfAndMonth()
        {
            // arrange
            var request = InternalFakes.GetBillingsRequests.ValidWithCpfAndMonth().Generate();
            var sut = new GetBillingsRequestValidator();

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
            var request = InternalFakes.GetBillingsRequests.InvalidCpf().Generate();
            var sut = new GetBillingsRequestValidator();

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidMonth()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.GetBillingsRequests.InvalidMonth().Generate();
            var sut = new GetBillingsRequestValidator();

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidWithoutFilters()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.GetBillingsRequests.InvalidEmpty().Generate();
            var sut = new GetBillingsRequestValidator();

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
