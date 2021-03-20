using Customers.Api.Domain.Models;
using FluentAssertions;
using Library.Results;
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
            const ulong expectedCpf = 01234567898;
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
            const ulong expectedCpf = 01234567898;
            const string expectedName = "sample";
            const string expectedState = "sample";
            var sut = new Customer
            {

                // act
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
