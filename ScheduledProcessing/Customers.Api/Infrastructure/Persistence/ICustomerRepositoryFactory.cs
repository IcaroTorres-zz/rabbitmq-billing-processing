using Customers.Api.Application.Abstractions;

namespace Customers.Api.Infrastructure.Persistence
{
    public interface ICustomerRepositoryFactory
    {
        ICustomerRepository CreateRepository();
    }
}
