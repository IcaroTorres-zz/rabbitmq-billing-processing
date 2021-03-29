using Library.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Application.Abstractions
{
    /// <summary>
    /// Represents a transactional unit of work capable of ACID commits and rollbacks
    /// </summary>
    public interface IUnitofwork : IDisposable
    {
        /// <summary>
        /// Checks if this instance has a transaction open
        /// </summary>
        /// <returns></returns>
        bool HasTransactionOpen();

        /// <summary>
        /// Begins a new transaction and return It wrapped by a unit of work
        /// </summary>
        /// <returns></returns>
        IUnitofwork BeginTransaction();

        /// <summary>
        /// Try commiting all operations under an open transaction, automatically rolling back
        /// changes in case of failures returning <see cref="IResult"/> for given operation
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IResult> CommitAsync(CancellationToken cancellationToken = default);
    }
}
