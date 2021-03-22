using BillingProcessing.Api.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace BillingProcessing.Api.Infrastructure.Persistence
{
    public static class QueryFilters
    {
        public static FilterDefinition<Billing> BillingsByCustomerCpf(ulong cpf)
        {
            return Builders<Billing>.Filter.Eq(x => x.Cpf, cpf);
        }

        public static FilterDefinition<Billing> BillingsDueDateInRange(DateTime startDate, DateTime endDate)
        {
            return Builders<Billing>.Filter.Gte(x => x.DueDateTime, startDate) &
                   Builders<Billing>.Filter.Lte(x => x.DueDateTime, endDate);
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

        public static FilterDefinition<Customer> CustomerActive()
        {
            return Builders<Customer>.Filter.Eq(x => x.Active, true);
        }
    }
}
