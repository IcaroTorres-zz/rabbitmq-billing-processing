﻿using Customers.Application.Validators;
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
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, false).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new RegisterCustomerRequestValidator(cpfValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_By_EmptyName()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.RegisterCustomerRequests.EmptyName().Generate();
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, false).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new RegisterCustomerRequestValidator(cpfValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_EmptyState()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.RegisterCustomerRequests.EmptyState().Generate();
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, false).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new RegisterCustomerRequestValidator(cpfValidator, repository);

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
            var request = InternalFakes.RegisterCustomerRequests.InvalidCpf().Generate();
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, false).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateFalse().Build();

            var sut = new RegisterCustomerRequestValidator(cpfValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }

        [Fact]
        public void Should_Fail_By_InvalidState()
        {
            // arrange
            const int expectedErrorsCount = 1;
            var request = InternalFakes.RegisterCustomerRequests.InvalidState().Generate();
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, false).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateFalse().Build();

            var sut = new RegisterCustomerRequestValidator(cpfValidator, repository);

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
            var repository = CustomerRepositoryMockBuilder.Create()
                .Exists(request.Cpf, true).Build();
            var cpfValidator = CpfValidatorMockBuilder.Create()
                .ValidateTrue().Build();

            var sut = new RegisterCustomerRequestValidator(cpfValidator, repository);

            // act
            var result = sut.Validate(request);

            // assert
            result.AssertValidationFailuresCount(expectedErrorsCount);
        }
    }
}