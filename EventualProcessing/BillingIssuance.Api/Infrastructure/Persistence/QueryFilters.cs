using BillingIssuance.Api.Domain.Models;
using MongoDB.Driver;

namespace BillingIssuance.Api.Infrastructure.Persistence
{
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
