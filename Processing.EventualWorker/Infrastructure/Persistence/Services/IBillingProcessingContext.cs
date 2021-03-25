using MongoDB.Driver;
using Processing.EventualWorker.Domain.Models;

namespace Processing.EventualWorker.Infrastructure.Persistence.Services
{
    public interface IBillingProcessingContext
    {
        IMongoCollection<Customer> Customers { get; }
        IMongoCollection<Billing> Billings { get; }
    }
}
