using Library.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Abstractions
{
    public interface IUnitofwork : IDisposable
    {
        bool HasTransactionOpen();
        IUnitofwork BeginTransaction();
        Task<IResult> CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
