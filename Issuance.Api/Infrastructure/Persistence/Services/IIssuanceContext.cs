using Issuance.Api.Domain.Models;
using MongoDB.Driver;

namespace Issuance.Api.Infrastructure.Persistence.Services
{
    public interface IIssuanceContext
    {
        IMongoCollection<Billing> Billings { get; }
    }
}
