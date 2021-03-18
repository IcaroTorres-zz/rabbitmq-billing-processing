using Customers.Api.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api.Infrastructure.Persistence
{
    /// <summary>
    /// Implementation for creating <see cref="ICustomerRepository"/> instances within singleton services.
    /// Depends on <see cref="DbContextOptions"/> which are usually injected automatically as a Scoped service,
    /// crashing application during startup and needs to be manually created as passed on constructor
    /// during startup dependency injection configuration.
    /// </summary>
    public interface ICustomerRepositoryFactory
    {
        /// <summary>
        /// Creates a new <see cref="ICustomerRepository"/> ready for I/O database operations
        /// </summary>
        /// <returns></returns>
        ICustomerRepository CreateRepository();
    }
}
