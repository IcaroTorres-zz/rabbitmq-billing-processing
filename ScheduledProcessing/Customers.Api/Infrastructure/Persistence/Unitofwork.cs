using Customers.Api.Application.Abstractions;
using Library.Abstractions;
using Library.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Infrastructure.Persistence
{
    public sealed class Unitofwork : IUnitofwork
    {
        private readonly CustomersContext context;
        private IDbContextTransaction transaction;
        private bool disposed;
        private bool transactionOpen = false;

        public Unitofwork(CustomersContext context)
        {
            this.context = context;
        }

        public bool HasTransactionOpen() => transactionOpen;

        public IUnitofwork BeginTransaction()
        {
            transaction = context.Database.BeginTransaction();
            transactionOpen = true;
            return this;
        }

        public async Task<IResult> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var changes = await context.SaveChangesAsync(cancellationToken);
                transaction?.CommitAsync(cancellationToken);
                transactionOpen = false;
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
            await transaction?.RollbackAsync(cancellationToken);
            transactionOpen = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                context.Dispose();
                transaction?.Dispose();
            }
            transactionOpen = false;
            disposed = true;
        }
    }
}
