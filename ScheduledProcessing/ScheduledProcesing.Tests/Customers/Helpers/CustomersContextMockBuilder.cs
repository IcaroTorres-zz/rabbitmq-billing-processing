using Customers.Api.Domain.Models;
using Customers.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ScheduledProcesing.Tests.Customers.Helpers
{
    public sealed class CustomersContextMockBuilder
    {
        private readonly Mock<CustomersContext> _mock;

        private CustomersContextMockBuilder()
        {
            _mock = new Mock<CustomersContext>();
        }

        public static CustomersContextMockBuilder Create()
        {
            return new CustomersContextMockBuilder();
        }

        public CustomersContextMockBuilder DatabaseBeginTransaction()
        {
            var transaction = new Mock<IDbContextTransaction>().Object;
            var databaseMock = new Mock<DatabaseFacade>(_mock.Object);
            databaseMock.Setup(x => x.BeginTransaction()).Returns(transaction);
            _mock.Setup(x => x.Database).Returns(databaseMock.Object);
            return this;
        }

        public CustomersContextMockBuilder Customers(params Customer[] customers)
        {
            var customersMock = customers.AsQueryable().BuildMockDbSet();
            _mock.Setup(x => x.Customers).Returns(customersMock.Object);
            _mock.Setup(x => x.Set<Customer>()).Returns(customersMock.Object);
            return this;
        }

        public CustomersContextMockBuilder AddCustomer(Customer customer)
        {
            _mock.Setup(x => x.Customers.AddAsync(customer, default))
                .ReturnsAsync(It.IsAny<EntityEntry<Customer>>())
                .Verifiable();
            return this;
        }

        public CustomersContextMockBuilder SaveChanges(int changes)
        {
            _mock.Setup(x => x.SaveChanges()).Returns(changes);
            _mock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(changes);

            return this;
        }

        public CustomersContextMockBuilder SaveChangesException(Exception ex)
        {
            _mock.Setup(x => x.SaveChanges()).Throws(ex);
            _mock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(ex);

            return this;
        }

        public CustomersContext Build()
        {
            return _mock.Object;
        }

        public CustomersContextMockBuilder Verify(Expression<Action<CustomersContext>> verify, Times times)
        {
            _mock.Verify(verify, times);

            return this;
        }
    }
}
