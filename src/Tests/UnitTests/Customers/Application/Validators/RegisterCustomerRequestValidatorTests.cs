using Customers.Application.Validators;
using Customers.Domain.Models;
using FluentAssertions;
using Library.TestHelpers;
using UnitTests.Customers.Helpers;
using Xunit;

namespace UnitTests.Customers.Application.Validators
{
    [Trait("customers", "application")]
    public class RegisterCustomerRequestValidatorTests
    {
        [Fact]
        public void Should_Succeed()
        {
            // arrange
            var request = InternalFakes.RegisterCustomerRequests.Valid().Generate();
            var factory = ModelFactoryMockBuilder.Create().CreateCustomer(request.Cpf, request.Name, request.State, InternalFakes.Customers.Valid().Generate()).Build();
            var customerValidator = ValidatorMockBuilder<Customer>.Create().ValidateTrue().Build();
            var repository = CustomerRepositoryMockBuilder.Create().Exists(request.Cpf, false).Build();
            var sut = new RegisterCustomerRequestValidator(factory, customerValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_By_InvalidCustomer()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.RegisterCustomerRequests.Invalid().Generate();
            var factory = ModelFactoryMockBuilder.Create().CreateCustomer(request.Cpf, request.Name, request.State, InternalFakes.Customers.InvalidState().Generate()).Build();
            var customerValidator = ValidatorMockBuilder<Customer>.Create().ValidateFalse().Build();
            var repository = CustomerRepositoryMockBuilder.Create().Exists(request.Cpf, false).Build();
            var sut = new RegisterCustomerRequestValidator(factory, customerValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_CustomerAlreadyExists()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.RegisterCustomerRequests.Valid().Generate();
            var factory = ModelFactoryMockBuilder.Create().CreateCustomer(request.Cpf, request.Name, request.State, InternalFakes.Customers.Valid().Generate()).Build();
            var customerValidator = ValidatorMockBuilder<Customer>.Create().ValidateTrue().Build();
            var repository = CustomerRepositoryMockBuilder.Create().Exists(request.Cpf, true).Build();
            var sut = new RegisterCustomerRequestValidator(factory, customerValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}
