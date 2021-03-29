using Customers.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Customers.Infrastructure.Persistence
{
    ///<inheritdoc cref="ICustomerRepositoryFactory"/>
    public class CustomerRepositoryFactory : ICustomerRepositoryFactory
    {
        private readonly DbContextOptions<CustomersContext> _options;

        public CustomerRepositoryFactory(DbContextOptions<CustomersContext> options)
        {
            _options = options;
        }
        public ICustomerRepository CreateRepository()
        {
            var context = new CustomersContext(_options);
            return new CustomerRepository(context);
        }
    }
}
