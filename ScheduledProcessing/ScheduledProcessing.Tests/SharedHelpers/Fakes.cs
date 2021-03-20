using Bogus;
using Library.ValueObjects;
using System.Collections.Generic;

namespace ScheduledProcessing.Tests.SharedHelpers
{
    public static class Fakes
    {
        public static class CPFs
        {
            public static Faker<CPF> Valid => new Faker<CPF>()
                .CustomInstantiator(_ => CPF.NewCPF());
            public static readonly CPF Invalid = CPF.From(0);
        }

        public static class States
        {
            private static readonly List<string> states = new List<string>()
            {
                "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
                "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
                "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
            };
            public static readonly string Valid = new Faker().PickRandom(states);
            public static readonly string Invalid = "AAA";
        }

        public static class Names
        {
            public static readonly string Valid = new Faker().Name.FullName();
            public static readonly string Empty = "";
        }
    }
}
