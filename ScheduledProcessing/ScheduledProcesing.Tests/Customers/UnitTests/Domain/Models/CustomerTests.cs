using Customers.Api.Domain.Models;
using FluentAssertions;
using Library.Results;
using ScheduledProcesing.Tests.SharedHelpers;
using Xunit;

namespace ScheduledProcesing.Tests.Customers.UnitTests.Domain.Models
{
    [Trait("unit-test", "customers-domain")]
    public class CustomerTests
    {
        [Fact]
        public void Constructor_Should_Create_With_EmptyProps()
        {
            // arrange and act
            var sut = new Customer();

            // assert
            sut.Should().NotBeNull().And.BeOfType<Customer>().And.NotBeAssignableTo<INull>();
            sut.Cpf.Should().Be(0);
            sut.Name.Should().BeNullOrEmpty();
            sut.State.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Constructor_AssigningValues_Should_Create_With_Props_For_GivenValues()
        {
            // arrange and act
            var expectedCpf = Fakes.CPFs.Valid.Generate();
            const string expectedName = "sample";
            const string expectedState = "sample";

            var sut = new Customer
            {
                Cpf = expectedCpf,
                Name = expectedName,
                State = expectedState
            };

            sut.Should().NotBeNull().And.BeOfType<Customer>().And.NotBeAssignableTo<INull>();
            sut.Cpf.Should().Be(expectedCpf);
            sut.Name.Should().NotBeNullOrEmpty().And.Be(expectedName);
            sut.State.Should().NotBeNullOrEmpty().And.Be(expectedState);
        }

        [Fact]
        public void Instance_Should_Have_Mutable_PropValues()
        {
            // arrange
            var expectedCpf = Fakes.CPFs.Valid.Generate();
            const string expectedName = "sample";
            const string expectedState = "sample";

            // act
            var sut = new Customer
            {
                Cpf = expectedCpf,
                Name = expectedName,
                State = expectedState
            };

            sut.Should().NotBeNull().And.BeOfType<Customer>().And.NotBeAssignableTo<INull>();
            sut.Cpf.Should().Be(expectedCpf);
            sut.Name.Should().NotBeNullOrEmpty().And.Be(expectedName);
            sut.State.Should().NotBeNullOrEmpty().And.Be(expectedState);
        }
    }
}
