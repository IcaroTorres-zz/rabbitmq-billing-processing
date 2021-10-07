using Billings.Application.Models;
using Billings.Domain.Models;
using Bogus;
using System;
using static Library.TestHelpers.Fakes;

namespace UnitTests.Billings.Helpers
{
    public static class InternalFakes
    {
        public static class Billings
        {
            public static Faker<Billing> Valid() => new Faker<Billing>()
                .RuleFor(x => x.Id, Guid.NewGuid)
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate())
                .RuleFor(x => x.Amount, x => x.Random.Double(1, 2000))
                .RuleFor(x => x.DueDate, Dates.Future(1));

            public static Faker<Billing> Processed() => Valid()
                .RuleFor(x => x.ProcessedAt, DateTime.UtcNow);
        }

        public static class Dates
        {
            public static Faker<Date> Future(int days) => new Faker<Date>()
                .CustomInstantiator(x =>
                {
                    var date = x.Date.Soon(days);
                    return new Date
                    {
                        Day = (byte)date.Day,
                        Month = (byte)date.Month,
                        Year = (ushort)date.Year
                    };
                });

            public static Faker<Date> Past() => new Faker<Date>()
                .CustomInstantiator(x =>
                {
                    var date = x.Date.Past(2);
                    return new Date
                    {
                        Day = (byte)date.Day,
                        Month = (byte)date.Month,
                        Year = (ushort)date.Year
                    };
                });
        }

        public static class BillingRequests
        {
            public static Faker<BillingRequest> Valid() => new Faker<BillingRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate().ToString())
                .RuleFor(x => x.Amount, x => x.Random.Double(10, 2000))
                .RuleFor(x => x.DueDate, Dates.Future(5).Generate().ToString());

            public static Faker<BillingRequest> InvalidCpf() => Valid()
                .RuleFor(x => x.Cpf, CPFs.Invalid.ToString());

            public static Faker<BillingRequest> InvalidAmount() => Valid()
                .RuleFor(x => x.Amount, -10);

            public static Faker<BillingRequest> InvalidDueDate() => Valid()
                .RuleFor(x => x.DueDate, Dates.Past().Generate().ToString());
        }

        public static class GetBillingsRequests
        {
            public static Faker<GetBillingsRequest> ValidWithCpf() => new Faker<GetBillingsRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate().ToString());

            public static Faker<GetBillingsRequest> ValidWithMonth() => new Faker<GetBillingsRequest>()
                .RuleFor(x => x.Month, Dates.Past().Generate().ToString()[3..]);

            public static Faker<GetBillingsRequest> ValidWithCpfAndMonth() => ValidWithCpf()
                .RuleFor(x => x.Month, Dates.Past().Generate().ToString()[3..]);

            public static Faker<GetBillingsRequest> InvalidEmpty() => new Faker<GetBillingsRequest>()
                .CustomInstantiator(_ => new GetBillingsRequest());

            public static Faker<GetBillingsRequest> InvalidCpf() => InvalidEmpty()
                .RuleFor(x => x.Cpf, CPFs.Invalid.ToString());

            public static Faker<GetBillingsRequest> InvalidMonth() => InvalidEmpty()
                .RuleFor(x => x.Month, "000");
        }

        public static class ProcessedBatchs
        {
            public static Faker<ProcessedBatch> Processed(int count = 5) => new Faker<ProcessedBatch>()
                .CustomInstantiator(x => new ProcessedBatch(Billings.Processed().Generate(count)));
        }
    }
}
