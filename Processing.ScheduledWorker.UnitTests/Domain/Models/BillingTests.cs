using FluentAssertions;
using Library.Results;
using Library.TestHelpers;
using Processing.ScheduledWorker.Domain.Models;
using System;
using Xunit;

namespace Processing.ScheduledWorker.UnitTests.Domain.Models
{
    [Trait("unit-test", "Processing.ScheduledWorker-domain")]
    public class BillingTests
    {
        [Fact]
        public void Constructor_Should_Create_With_EmptyProps()
        {
            // arrange and act
            var sut = new Billing();

            // assert
            sut.Should().NotBeNull().And.BeOfType<Billing>().And.NotBeAssignableTo<INull>();
            sut.Id.Should().BeEmpty();
            sut.Cpf.Should().Be(0);
            sut.Amount.Should().Be(0);
        }

        [Fact]
        public void Constructor_AssigningValues_Should_Create_With_Props_For_GivenValues()
        {
            // arrange and act
            var expectedCpf = Fakes.CPFs.Valid().Generate();
            const decimal expectedAmount = 12345;
            var expectedId = Guid.NewGuid();

            var sut = new Billing
            {
                Id = expectedId,
                Cpf = expectedCpf,
                Amount = expectedAmount
            };

            sut.Should().NotBeNull().And.BeOfType<Billing>().And.NotBeAssignableTo<INull>();
            sut.Id.Should().Be(expectedId);
            sut.Cpf.Should().Be(expectedCpf);
            sut.Amount.Should().Be(expectedAmount);
        }

        [Fact]
        public void Instance_Should_Have_Mutable_PropValues()
        {
            // arrange and act
            var sut = new Billing();
            var previousId = sut.Id;
            var previousCpf = sut.Cpf;
            var previousAmount = sut.Amount;
            var expectedId = Guid.NewGuid();
            var expectedCpf = Fakes.CPFs.Valid().Generate();
            const decimal expectedAmount = 12345;

            // act
            sut.Id = expectedId;
            sut.Cpf = expectedCpf.Value;
            sut.Amount = expectedAmount;

            sut.Should().NotBeNull().And.BeOfType<Billing>().And.NotBeAssignableTo<INull>();
            sut.Id.Should().Be(expectedId).And.NotBe(previousId);
            sut.Cpf.Should().Be(expectedCpf.Value).And.NotBe(previousCpf);
            sut.Amount.Should().Be(expectedAmount).And.NotBe(previousAmount);
        }
    }
}
