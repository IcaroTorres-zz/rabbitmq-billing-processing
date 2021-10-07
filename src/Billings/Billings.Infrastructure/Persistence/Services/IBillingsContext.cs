using Billings.Domain.Models;
using MongoDB.Driver;

namespace Billings.Infrastructure.Persistence.Services
{
    public interface IBillingsContext
    {
        IMongoCollection<Billing> Billings { get; }
    }
}
