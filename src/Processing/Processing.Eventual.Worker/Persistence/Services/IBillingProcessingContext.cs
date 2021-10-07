using MongoDB.Driver;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Worker.Persistence.Services
{
    public interface IBillingProcessingContext
    {
        IMongoCollection<Customer> Customers { get; }
        IMongoCollection<Billing> Billings { get; }
    }
}
