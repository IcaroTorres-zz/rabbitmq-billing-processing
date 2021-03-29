using Billings.Domain.Models;
using MongoDB.Driver;

namespace Billings.Infrastructure.Persistence
{
    /// <summary>
    /// Predefined command rule specifications
    /// </summary>

    public static class CommandDefinitions
    {
        public static UpdateDefinition<Billing> SetProcessed(Billing entity)
        {
            return Builders<Billing>.Update
                .Set(x => x.ProcessedAt, entity.ProcessedAt)
                .Set(x => x.Amount, entity.Amount);
        }
    }
}
