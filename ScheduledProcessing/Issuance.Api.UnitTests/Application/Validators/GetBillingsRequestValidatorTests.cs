﻿using FluentAssertions;
using Issuance.Api.Application.Validators;
using Issuance.Api.UnitTests.Helpers;
using Library.TestHelpers;
using Xunit;

namespace Issuance.Api.UnitTests.Application.Validators
{
    [Trait("unit-test", "issuance.api-application")]
    public class GetBillingsRequestValidatorTests
    {
        [Fact]
        public void Should_Succeed_WithValidCpf()
        {
            // arrange
            var request = InternalFakes.GetBillingsRequests.ValidWithCpf().Generate();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new GetBillingsRequestValidator(cpfValidator);

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
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new GetBillingsRequestValidator(cpfValidator);

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
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new GetBillingsRequestValidator(cpfValidator);

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
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateFalse().Build();

            var sut = new GetBillingsRequestValidator(cpfValidator);

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
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new GetBillingsRequestValidator(cpfValidator);

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
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new GetBillingsRequestValidator(cpfValidator);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
