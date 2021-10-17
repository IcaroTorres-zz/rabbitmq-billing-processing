using Billings.Application.Validators;
using Billings.Domain.Models;
using FluentAssertions;
using Library.TestHelpers;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Application.Validators
{
    [Trait("billings", "application")]
    public class BillingRequestValidatorTests
    {
        [Fact]
        public void Should_Succeed_WithValidRequest()
        {
            // arrange
            var request = InternalFakes.BillingRequests.Valid().Generate();
            var billing = InternalFakes.Billings.Valid().Generate();
            var billingValidator = ValidatorMockBuilder<Billing>.Create().ValidateTrue().Build();
            var factory = ModelFactoryMockBuilder.Create().CreateBilling(request.Cpf, request.Amount, request.DueDate, billing).Build();
            var sut = new BillingRequestValidator(factory, billingValidator);

            // act
            var result = sut.Validate(request);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_By_InvalidBilling()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.BillingRequests.InvalidAmount().Generate();
            var billingValidator = ValidatorMockBuilder<Billing>.Create().ValidateFalse().Build();
            var factory = ModelFactoryMockBuilder.Create().CreateBilling(request.Cpf, request.Amount, request.DueDate, new Billing()).Build();
            var sut = new BillingRequestValidator(factory, billingValidator);

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
            var billingValidator = ValidatorMockBuilder<Billing>.Create().ValidateTrue().Build();
            var factory = ModelFactoryMockBuilder.Create().CreateBilling(request.Cpf, request.Amount, request.DueDate, new Billing()).Build();
            var sut = new BillingRequestValidator(factory, billingValidator);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
