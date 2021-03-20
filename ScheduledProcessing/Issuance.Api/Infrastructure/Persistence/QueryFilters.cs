using Issuance.Api.Domain.Models;
using MongoDB.Driver;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Issuance.Api.Infrastructure.Persistence
{
    /// <summary>
    /// Predefined query rule specifications able to be combined more complex ones
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class QueryFilters
    {
        public static FilterDefinition<Billing> ByCustomerCpf(ulong cpf)
        {
            return cpf == 0
                ? FilterDefinition<Billing>.Empty
                : Builders<Billing>.Filter.Eq(x => x.Cpf, cpf);
        }

        public static FilterDefinition<Billing> ByMonthYear(byte month, ushort year)
        {
            return month == 0 || year == 0
                ? FilterDefinition<Billing>.Empty
                : Builders<Billing>.Filter.Eq(x => x.DueDate.Month, month) &
                  Builders<Billing>.Filter.Eq(x => x.DueDate.Year, year);
        }

        public static FilterDefinition<Billing> PendingProcessment()
        {
            return Builders<Billing>.Filter.Eq(x => x.ProcessedAt, null);
        }

        public static FilterDefinition<Billing> ById(Guid id)
        {
            return Builders<Billing>.Filter.Eq(x => x.Id, id);
        }
    }
}
