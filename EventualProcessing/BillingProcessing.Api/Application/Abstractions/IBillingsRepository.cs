using BillingProcessing.Api.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Application.Abstractions
{
    public interface IBillingsRepository
    {
        Task<List<Billing>> GetCustomerPendingBillingsAsync(ulong id, CancellationToken token);
        Task<Billing> InsertAsync(Billing entity, CancellationToken token);
        Task UpdateManyProcessedAsync(IEnumerable<Billing> entities, CancellationToken token);
        Task<List<Billing>> GetCustomerProcessedBillingsAsync(ulong customerId, DateTime startDate, DateTime endDate, CancellationToken token);
    }
}
