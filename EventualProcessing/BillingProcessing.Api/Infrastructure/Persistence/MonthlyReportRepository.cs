using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Responses;
using BillingProcessing.Api.Domain.Models;
using BillingProcessing.Api.Infrastructure.Persistence.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BillingProcessing.Api.Infrastructure.Persistence
{
    public class MonthlyReportRepository : IMonthlyReportRepository
    {
        private readonly IBillingProcessingContext context;

        public MonthlyReportRepository(IBillingProcessingContext context)
        {
            this.context = context;
        }

        public async Task<List<MonthlyReportResponse>> MapReduceMonthlyBillingByStateAsync(CancellationToken token)
        {
            var map = new BsonJavaScript(@"
            function() {
                var key = `${this.DueDate.Month}-${this.DueDate.Year}`;
                var value = { Amount: this.Amount, State: this.CustomerState }
                emit(key, value);
            }");

            var reduce = new BsonJavaScript(@"
            function(monthYear, values) {
                var total = 0
                var statesObject = values.reduce(function(accumulator, item) {
                    total += item.Amount
                    if (!!accumulator[item.State]) {
                        accumulator[item.State].Total += item.Amount;
                        return accumulator;
                    }
                    accumulator[item.State] = { State: item.State, Total: item.Amount };
                }, { Total: 0 });

                return { Total: total, States: Object.values(statesObject) };
            }");

            var options = new MapReduceOptions<Billing, MonthlyReportResponse> { Finalize = @"
            function (key, reducedVal) {
                return { Month: key, Total: reducedVal.Total, States: reducedVal.States };
            }" };
            var monthlyReportCursor = await context.Billings.MapReduceAsync(map, reduce, options, token);
            return await monthlyReportCursor.ToListAsync(token);
        }
    }
}
