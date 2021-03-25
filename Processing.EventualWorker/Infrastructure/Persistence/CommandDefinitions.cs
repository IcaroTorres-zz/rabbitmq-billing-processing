﻿using MongoDB.Driver;
using Processing.EventualWorker.Domain.Models;

namespace Processing.EventualWorker.Infrastructure.Persistence
{
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
