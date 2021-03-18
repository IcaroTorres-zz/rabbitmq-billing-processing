using Customers.Api.Application.Abstractions;
using Library.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Infrastructure.Persistence
{
    /// <inheritdoc cref="IUnitofwork" />
    public sealed class Unitofwork : IUnitofwork
    {
        private readonly CustomersContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed;
        private bool _transactionOpen = false;

        public Unitofwork(CustomersContext context)
        {
            _context = context;
        }

        public bool HasTransactionOpen() => _transactionOpen;

        public IUnitofwork BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            _transactionOpen = true;
            return this;
        }

        public async Task<IResult> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var changes = await _context.SaveChangesAsync(cancellationToken);
                _transaction?.CommitAsync(cancellationToken);
                _transactionOpen = false;
                return new SuccessResult(changes);
            }
            catch (Exception exception)
            {
                return new FailResult(StatusCodes.Status409Conflict,
                    new DbUpdateException("Failed handling data layer operation.", exception).ExtractMessages());
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _transaction?.RollbackAsync(cancellationToken);
            _transactionOpen = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            _transactionOpen = false;
            _disposed = true;
        }
    }
}
