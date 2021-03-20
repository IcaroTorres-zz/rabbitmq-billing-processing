﻿using Customers.Api.Application.Responses;
using Customers.Api.Application.Usecases;
using FluentAssertions;
using Library.Results;
using Microsoft.AspNetCore.Http;
using ScheduledProcesing.Tests.Customers.Helpers;
using ScheduledProcesing.Tests.SharedHelpers;
using System.Threading.Tasks;
using Xunit;

namespace ScheduledProcesing.Tests.Customers.UnitTests.Application.Usecases
{
    [Trait("unit-test", "customers-application")]
    public class RegisterCustomerUsecaseTests
    {
        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommand()
        {
            // arrange
            var request = Fakes.RegisterCustomerRequests.Valid().Generate();

            const int expectedStatus = StatusCodes.Status201Created;
            const int expectedChanges = 1;
            var expectedCommitResult = new SuccessResult(expectedChanges);
            var unitofwork = UnitofworkMockBuilder.Create()
                .BeginTransaction()
                .Commit(expectedCommitResult).Build();

            var expectedCustomer = Fakes.Customers.Valid().Generate();
            var expectedCpf = expectedCustomer.Cpf.ToString("00000000000");
            var factory = ModelFactoryMockBuilder.Create().CreateCustomer(
                cpfString: request.Cpf,
                name: request.Name,
                state: request.State,
                model: expectedCustomer).Build();

            var repository = CustomerRepositoryMockBuilder.Create()
                .Insert(expectedCustomer, Task.CompletedTask).Build();

            var sut = new RegisterCustomerUsecase(unitofwork, factory, repository);

            // act
            var result = await sut.Handle(request, default);
            var resultData = result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<CreatedWithLocationResult<CustomerResponse>>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(expectedStatus);
            resultData.Should().NotBeNull().And.BeOfType<CustomerResponse>();
            resultData.As<CustomerResponse>().Cpf.Should().Be(expectedCpf);
            resultData.As<CustomerResponse>().Name.Should().Be(expectedCustomer.Name);
            resultData.As<CustomerResponse>().State.Should().Be(expectedCustomer.State);
        }

        [Fact]
        public async Task Handle_Should_Fail_With_Commit_FailResult()
        {
            // arrange
            var request = Fakes.RegisterCustomerRequests.Valid().Generate();

            const int expectedStatus = StatusCodes.Status409Conflict;
            var expectedErrors = new[] { "dummy expected error message" };
            var expectedCommitResult = new FailResult(expectedStatus, expectedErrors);
            var unitofwork = UnitofworkMockBuilder.Create()
                .BeginTransaction()
                .Commit(expectedCommitResult).Build();

            var expectedCustomer = Fakes.Customers.Valid().Generate();
            var factory = ModelFactoryMockBuilder.Create().CreateCustomer(
                cpfString: request.Cpf,
                name: request.Name,
                state: request.State,
                model: expectedCustomer).Build();

            var repository = CustomerRepositoryMockBuilder.Create()
                .Insert(expectedCustomer, Task.CompletedTask).Build();

            var sut = new RegisterCustomerUsecase(unitofwork, factory, repository);

            // act
            var result = await sut.Handle(request, default);
            var resultData = result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<FailResult>();
            result.IsSuccess().Should().BeFalse();
            result.Errors.Should().NotBeEmpty().And.BeEquivalentTo(expectedErrors);
            result.GetStatus().Should().Be(expectedStatus);
            resultData.Should().BeNull();
        }
    }
}
