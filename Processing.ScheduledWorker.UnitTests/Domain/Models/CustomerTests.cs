using FluentAssertions;
using Library.Results;
using Library.TestHelpers;
using Processing.ScheduledWorker.Domain.Models;
using Xunit;

namespace Processing.ScheduledWorker.UnitTests.Domain.Models
{
    [Trait("unit-test", "Processing.ScheduledWorker-domain")]
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
        }

        [Fact]
        public void Constructor_AssigningValues_Should_Create_With_Props_For_GivenValues()
        {
            // arrange and act
            var expectedCpf = Fakes.CPFs.Valid().Generate();

            var sut = new Customer
            {
                Cpf = expectedCpf
            };

            sut.Should().NotBeNull().And.BeOfType<Customer>().And.NotBeAssignableTo<INull>();
            sut.Cpf.Should().Be(expectedCpf);
        }

        [Fact]
        public void Instance_Should_Have_Mutable_PropValues()
        {
            // arrange
            var sut = new Customer();
            var previousCpf = sut.Cpf;
            var expectedCpf = Fakes.CPFs.Valid().Generate();

            // act
            sut.Cpf = expectedCpf;

            sut.Should().NotBeNull().And.BeOfType<Customer>().And.NotBeAssignableTo<INull>();
            sut.Cpf.Should().Be(expectedCpf).And.NotBe(previousCpf);
        }
    }
}
