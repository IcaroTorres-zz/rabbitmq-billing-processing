using Issuance.Api.Domain.Models;
using Issuance.Api.Domain.Services;
using FluentAssertions;
using Library.Results;
using ScheduledProcessing.Tests.SharedHelpers;
using Xunit;
using Bogus;
using ScheduledProcessing.Tests.Issuance.Helpers;

namespace ScheduledProcessing.Tests.Issuance.UnitTests.Domain.Services
{
    [Trait("unit-test", "customers-domain")]
    public class ModelFactoryTests
    {
        [Fact]
        public void CreateBilling_Should_Create_Billing_With_PropsMatching_InUpperCase()
        {
            // arrange
            var sut = new ModelFactory();
            var expectedCpf = Fakes.CPFs.Valid.Generate();
            var expectedAmount = new Faker().Random.Double(10, 10000);
            var expectedDate = InternalFakes.Dates.Future(5).Generate();

            // act
            var result = sut.CreateBilling(expectedCpf.ToString(), expectedAmount, expectedDate.ToString());

            // assert
            result.Should().NotBeNull().And.BeOfType<Billing>().And.NotBeAssignableTo<INull>();
            result.Id.Should().NotBeEmpty();
            result.Cpf.Should().Be(expectedCpf.Value);
            result.Amount.Should().Be(expectedAmount);
            result.DueDate.Should().NotBeNull();
            result.DueDate.ToString().Should().Be(expectedDate.ToString());
        }
    }
}
