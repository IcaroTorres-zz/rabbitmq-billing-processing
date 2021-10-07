using Customers.Application.Abstractions;
using Customers.Domain.Models;
using Library.Optimizations;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Customers.Helpers
{
    public sealed class CustomerRepositoryMockBuilder
    {
        private readonly Mock<ICustomerRepository> _mock;

        private CustomerRepositoryMockBuilder()
        {
            _mock = new Mock<ICustomerRepository>();
        }

        public static CustomerRepositoryMockBuilder Create()
        {
            return new CustomerRepositoryMockBuilder();
        }

        public CustomerRepositoryMockBuilder Exists(string cpfString, bool result)
        {
            var cpf = cpfString.AsSpan().ParseUlong();
            _mock.Setup(x => x.ExistAsync(cpf, default)).ReturnsAsync(result);
            return this;
        }

        public CustomerRepositoryMockBuilder Get(string cpfString, Customer result)
        {
            var cpf = cpfString.AsSpan().ParseUlong();
            _mock.Setup(x => x.GetAsync(cpf, default)).ReturnsAsync(result);
            return this;
        }

        public CustomerRepositoryMockBuilder GetAll(List<Customer> result)
        {
            _mock.Setup(x => x.GetAllAsync()).ReturnsAsync(result);
            return this;
        }

        public CustomerRepositoryMockBuilder Insert(Customer entity, Task result)
        {
            _mock.Setup(x => x.InsertAsync(entity, default)).Returns(result);
            return this;
        }

        public ICustomerRepository Build()
        {
            return _mock.Object;
        }
    }
}
