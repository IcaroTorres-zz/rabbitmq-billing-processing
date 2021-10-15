using Bogus;
using Library.ValueObjects;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;

namespace Library.TestHelpers
{
    public static class Fakes
    {
        public static class CPFs
        {
            public static Faker<Cpf> Valid() => new Faker<Cpf>()
                .CustomInstantiator(_ => Cpf.NewCpf());
            public static readonly Cpf Invalid = Cpf.From(0);
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

        public static class DeliverEventArgs
        {
            public static Faker<BasicDeliverEventArgs> WithBody<T>(T value)
            {
                return new Faker<BasicDeliverEventArgs>().CustomInstantiator(x =>
                {
                    var stringValue = JsonConvert.SerializeObject(value);
                    var bytes = Encoding.UTF8.GetBytes(stringValue);
                    return new BasicDeliverEventArgs { Body = bytes };
                });
            }
        }
    }
}
