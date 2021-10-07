using MongoDB.Driver;
using Processing.Eventual.Domain.Models;
using System.Collections.Generic;

namespace Processing.Eventual.Worker.Persistence
{
    public static class QueryFilters
    {
        public static FilterDefinition<Billing> BillingsByCustomerCpf(ulong cpf)
        {
            return Builders<Billing>.Filter.Eq(x => x.Cpf, cpf);
        }

        public static FilterDefinition<Billing> BillingsPending()
        {
            return Builders<Billing>.Filter.Eq(x => x.ProcessedAt, null);
        }

        public static FilterDefinition<Billing> BillingsProcessed()
        {
            return Builders<Billing>.Filter.Ne(x => x.ProcessedAt, null);
        }

        public static FilterDefinition<Billing> BillingById(string id)
        {
            return Builders<Billing>.Filter.Eq(x => x.Id, id);
        }

        public static FilterDefinition<Billing> BillingIdIn(IEnumerable<string> ids)
        {
            return Builders<Billing>.Filter.In(x => x.Id, ids);
        }

        public static FilterDefinition<Customer> CustomerByCpf(ulong cpf)
        {
            return Builders<Customer>.Filter.Eq(x => x.Cpf, cpf);
        }
    }
}
