using Bogus;
using Customers.Api.Application.Requests;
using Customers.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;

namespace ScheduledProcesing.Tests.SharedHelpers
{
    public static class Fakes
    {
        public static class CPFs
        {
            public static readonly ulong Valid = 77355245104;
            public static readonly ulong Invalid = 00000000000;
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

        public static class Customers
        {
            public static Faker<Customer> Valid() => new Faker<Customer>()
                .RuleFor(x => x.Cpf, CPFs.Valid)
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, States.Valid);

            public static Faker<Customer> InvalidCpf() => Valid()
                .RuleFor(x => x.Cpf, CPFs.Invalid);

            public static Faker<Customer> InvalidState() => Valid()
                .RuleFor(x => x.State, States.Invalid);

            public static Faker<Customer> EmptyName() => Valid()
                .RuleFor(x => x.Name, "");

            public static Faker<Customer> EmptyState() => Valid()
                .RuleFor(x => x.State, "");
        }

        public static class RegisterCustomerRequests
        {
            public static Faker<RegisterCustomerRequest> Valid() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.ToString("00000000000"))
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, States.Valid)
                .FinishWith((x, request) =>
                {
                    const string routename = "get-customer";
                    var urlHelperMock = new Mock<IUrlHelper>();
                    urlHelperMock.Setup(x => x.Link(routename, It.IsAny<object>())).Returns(x.Internet.Url());
                    var urlHelper = urlHelperMock.Object;
                    request.SetupForCreation(urlHelper, routename, c => new { c.Cpf });
                });

            public static Faker<RegisterCustomerRequest> InvalidCpf() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, States.Valid)
                .RuleFor(x => x.Cpf, CPFs.Invalid.ToString());

            public static Faker<RegisterCustomerRequest> InvalidState() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.ToString("00000000000"))
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, States.Invalid);

            public static Faker<RegisterCustomerRequest> EmptyName() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.ToString("00000000000"))
                .RuleFor(x => x.State, States.Valid)
                .RuleFor(x => x.Name, "");

            public static Faker<RegisterCustomerRequest> EmptyState() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.ToString("00000000000"))
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, "");
        }

        public static class GetCustomerRequests
        {
            public static Faker<GetCustomerRequest> Valid() => new Faker<GetCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.ToString("00000000000"));

            public static Faker<GetCustomerRequest> InvalidCpf() => Valid()
                .RuleFor(x => x.Cpf, CPFs.Invalid.ToString());
        }
    }
}
