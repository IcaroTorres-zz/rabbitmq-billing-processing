using Customers.Application.Abstractions;
using Customers.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.Customers.Infrastructure.Persistence
{
    [Trait("customers", "infrastructure")]
    public class CustomerRepositoryFactoryTests
    {
        [Fact]
        public void CreateRepository_Should_Create_RepositoryInstance()
        {
            // arrange
            var options = new DbContextOptionsBuilder<CustomersContext>()
                .UseInMemoryDatabase(databaseName: "CustomersContextInMemory")
                .Options;
            var sut = new CustomerRepositoryFactory(options);

            // act
            var result = sut.CreateRepository();

            // assert
            result.Should().NotBeNull()
                .And.BeOfType<CustomerRepository>()
                .And.BeAssignableTo<ICustomerRepository>();
        }
    }
}
