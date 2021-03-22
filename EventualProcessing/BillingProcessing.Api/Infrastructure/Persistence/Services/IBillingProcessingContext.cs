using BillingProcessing.Api.Domain.Models;
using MongoDB.Driver;

namespace BillingProcessing.Api.Infrastructure.Persistence.Services
{
    public interface IBillingProcessingContext
    {
        IMongoCollection<Customer> Customers { get; }
        IMongoCollection<Billing> Billings { get; }
    }
}
