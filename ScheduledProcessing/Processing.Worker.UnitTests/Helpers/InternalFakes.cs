using Bogus;
using Processing.Worker.Domain.Models;
using System;
using static Library.TestHelpers.Fakes;

namespace Processing.Worker.UnitTests.Helpers
{
    public static class InternalFakes
    {
        public static class Billings
        {
            public static Faker<Billing> Valid() => new Faker<Billing>()
                .RuleFor(x => x.Id, Guid.NewGuid)
                .RuleFor(x => x.Cpf, x => CPFs.Valid().Generate())
                .RuleFor(x => x.Amount, x => x.Random.Decimal(1, 2000));
        }

        public static class Customer
        {
            public static Faker<Billing> Valid() => new Faker<Billing>().RuleFor(x => x.Cpf, CPFs.Valid().Generate());
        }
    }
}
