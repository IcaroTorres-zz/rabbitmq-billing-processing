using Billings.Domain.Models;
using FluentAssertions;
using Library.Results;
using Library.TestHelpers;
using System;
using UnitTests.Billings.Helpers;
using Xunit;

namespace UnitTests.Billings.Domain.Models
{
    [Trait("issuance", "domain")]
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
            sut.DueDate.Should().BeNull();
            sut.Amount.Should().Be(0);
        }

        [Fact]
        public void Constructor_AssigningValues_Should_Create_With_Props_For_GivenValues()
        {
            // arrange and act
            var expectedCpf = Fakes.CPFs.Valid().Generate();
            const double expectedAmount = 12345;
            var expectedDate = InternalFakes.Dates.Future(1).Generate();
            var expectedId = Guid.NewGuid();

            var sut = new Billing
            {
                Id = expectedId,
                Cpf = expectedCpf,
                Amount = expectedAmount,
                DueDate = expectedDate
            };

            sut.Should().NotBeNull().And.BeOfType<Billing>().And.NotBeAssignableTo<INull>();
            sut.Id.Should().Be(expectedId);
            sut.Cpf.Should().Be(expectedCpf);
            sut.Amount.Should().Be(expectedAmount);
            sut.DueDate.Should().Be(expectedDate);
        }

        [Fact]
        public void Instance_Should_Have_Mutable_PropValues()
        {
            // arrange and act
            var sut = new Billing();
            var previousId = sut.Id;
            var previousCpf = sut.Cpf;
            var previousAmount = sut.Amount;
            var previousDueDate = sut.DueDate;
            var expectedId = Guid.NewGuid();
            var expectedCpf = Fakes.CPFs.Valid().Generate();
            const double expectedAmount = 12345;
            var expectedDate = InternalFakes.Dates.Future(1).Generate();

            // act
            sut.Id = expectedId;
            sut.Cpf = expectedCpf.Value;
            sut.Amount = expectedAmount;
            sut.DueDate = expectedDate;

            sut.Should().NotBeNull().And.BeOfType<Billing>().And.NotBeAssignableTo<INull>();
            sut.Id.Should().Be(expectedId).And.NotBe(previousId);
            sut.Cpf.Should().Be(expectedCpf.Value).And.NotBe(previousCpf);
            sut.Amount.Should().Be(expectedAmount).And.NotBe(previousAmount);
            sut.DueDate.Should().Be(expectedDate).And.NotBe(previousDueDate);
        }
    }
}
