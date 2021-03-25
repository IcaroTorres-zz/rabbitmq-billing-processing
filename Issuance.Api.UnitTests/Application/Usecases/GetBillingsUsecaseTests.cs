using FluentAssertions;
using Issuance.Api.Application.Models;
using Issuance.Api.Application.Usecases;
using Issuance.Api.UnitTests.Helpers;
using Library.Optimizations;
using Library.Results;
using Library.ValueObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Issuance.Api.UnitTests.Application.Usecases
{
    [Trait("unit-test", "issuance.api-application")]
    public class GetBillingUsecaseTests
    {
        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommandWithCpf()
        {
            // arrange
            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var expectedBillingsResponses = expectedBillings.ConvertAll(x => new BillingResponse(x));
            var request = InternalFakes.GetBillingsRequests.ValidWithCpf().Generate();
            var cpf = request.Cpf.AsSpan().ParseUlong();
            var repository = BillingRepositoryMockBuilder.Create()
                .GetMany(cpf, 0, 0, expectedBillings).Build();
            var sut = new GetBillingsUsecase(repository);

            // act
            var result = await sut.Handle(request, default);
            var resultData = (List<BillingResponse>)result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(StatusCodes.Status200OK);
            resultData.Should().NotBeNull().And.BeOfType<List<BillingResponse>>()
                .And.BeEquivalentTo(expectedBillingsResponses);
        }

        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommandWithMonth()
        {
            // arrange
            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var expectedBillingsResponses = expectedBillings.ConvertAll(x => new BillingResponse(x));
            var request = InternalFakes.GetBillingsRequests.ValidWithMonth().Generate();
            Date.TryParseMonth(request.Month, out var month, out var year);
            var repository = BillingRepositoryMockBuilder.Create()
                .GetMany(0, month, year, expectedBillings).Build();
            var sut = new GetBillingsUsecase(repository);

            // act
            var result = await sut.Handle(request, default);
            var resultData = (List<BillingResponse>)result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(StatusCodes.Status200OK);
            resultData.Should().NotBeNull().And.BeOfType<List<BillingResponse>>()
                .And.BeEquivalentTo(expectedBillingsResponses);
        }

        [Fact]
        public async Task Handle_Should_Succeed_With_ValidCommandWithCpfAndMonth()
        {
            // arrange
            var expectedBillings = InternalFakes.Billings.Valid().Generate(2);
            var expectedBillingsResponses = expectedBillings.ConvertAll(x => new BillingResponse(x));
            var request = InternalFakes.GetBillingsRequests.ValidWithCpfAndMonth().Generate();
            var cpf = request.Cpf.AsSpan().ParseUlong();
            Date.TryParseMonth(request.Month, out var month, out var year);
            var repository = BillingRepositoryMockBuilder.Create()
                .GetMany(cpf, month, year, expectedBillings).Build();
            var sut = new GetBillingsUsecase(repository);

            // act
            var result = await sut.Handle(request, default);
            var resultData = (List<BillingResponse>)result.GetData();

            // assert
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.GetStatus().Should().Be(StatusCodes.Status200OK);
            resultData.Should().NotBeNull().And.BeOfType<List<BillingResponse>>()
                .And.BeEquivalentTo(expectedBillingsResponses);
        }
    }
}
