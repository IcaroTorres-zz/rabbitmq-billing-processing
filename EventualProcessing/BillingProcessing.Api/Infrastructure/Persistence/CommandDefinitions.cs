using BillingProcessing.Api.Domain.Models;
using MongoDB.Driver;

namespace BillingProcessing.Api.Infrastructure.Persistence
{
    public static class CommandDefinitions
    {
        public static UpdateDefinition<Billing> SetProcessed(Billing entity)
        {
            return Builders<Billing>.Update
                .Set(x => x.ProcessedAt, entity.ProcessedAt)
                .Set(x => x.Amount, entity.Amount)
                .Set(x => x.CustomerState, entity.CustomerState);
        }

        public static UpdateDefinition<Customer> SetEnabled(Customer entity)
        {
            return Builders<Customer>.Update
                .Set(x => x.Active, entity.Active)
                .Set(x => x.State, entity.State);
        }
    }
}
