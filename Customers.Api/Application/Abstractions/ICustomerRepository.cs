using Customers.Api.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Abstractions
{
    /// <summary>
    /// A null-free memory abstraction for I/O database operations related to <see cref="Customer"/>
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Checks if there is a database <see cref="Customer"/> record with given key
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(ulong cpf, CancellationToken token);

        /// <summary>
        /// Gets a <see cref="Customer"/> record by given key
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Customer> GetAsync(ulong cpf, CancellationToken token);

        /// <summary>
        /// Inserts a new record to database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task InsertAsync(Customer entity, CancellationToken token);

        /// <summary>
        /// Gets a list of all available <see cref="Customer"/> records from database
        /// </summary>
        /// <returns></returns>
        Task<List<Customer>> GetAllAsync();
    }
}
