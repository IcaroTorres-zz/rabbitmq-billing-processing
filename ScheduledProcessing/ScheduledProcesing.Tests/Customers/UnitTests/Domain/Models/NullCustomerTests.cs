using Customers.Api.Domain.Models;
using FluentAssertions;
using Library.Results;
using Xunit;

namespace ScheduledProcesing.Tests.Customers.UnitTests.Domain.Models
{
    [Trait("unit-test", "customers-domain")]
    public class NullCustomerTests
    {
        [Fact]
        public void Constructor_Should_Create_With_EmptyProps()
        {
            // arrange and act
            var sut = new NullCustomer();

            // assert
            sut.Should().NotBeNull().And.NotBeOfType<Customer>().And.BeAssignableTo<INull>();
            sut.Cpf.Should().Be(0);
            sut.Name.Should().BeNullOrEmpty();
            sut.State.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Constructor_AssigningValues_Should_Create_With_EmptyProps()
        {
            // arrange and act
            var sut = new NullCustomer
            {
                Name = "sample",
                State = "sample",
                Cpf = 01234567890
            };

            sut.Should().NotBeNull().And.NotBeOfType<Customer>().And.BeAssignableTo<INull>();
            sut.Cpf.Should().Be(0);
            sut.Name.Should().BeNullOrEmpty();
            sut.State.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Instance_Should_Have_ImmutableProps()
        {
            // arrange and act
            var sut = new NullCustomer
            {
                Name = "sample",
                State = "sample",
                Cpf = 01234567890
            };

            sut.Should().NotBeNull().And.NotBeOfType<Customer>().And.BeAssignableTo<INull>();
            sut.Cpf.Should().Be(0);
            sut.Name.Should().BeNullOrEmpty();
            sut.State.Should().BeNullOrEmpty();
        }
    }
}
