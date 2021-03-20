using Bogus;
using Customers.Api.Application.Requests;
using Customers.Api.Domain.Models;
using Issuance.Api.Domain.Models;
using Library.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Date = Issuance.Api.Domain.Models.Date;

namespace ScheduledProcesing.Tests.SharedHelpers
{
    public static class Fakes
    {
        public static class CPFs
        {
            public static Faker<CPF> Valid => new Faker<CPF>()
                .CustomInstantiator(_ =>  CPF.NewCPF());
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

        public static class DateStrings
        {
            public static Faker<string> Future(int days) => new Faker<string>()
                .CustomInstantiator(x => x.Date.Soon(days).ToString("dd-MM-yyyy"));

            public static Faker<string> Past() => new Faker<string>()
                .CustomInstantiator(x => x.Date.Past(2).ToString("dd-MM-yyyy"));

            public static Faker<string> InvalidFormat() => new Faker<string>()
                .CustomInstantiator(x => x.Date.Past(2).ToString("ddd-MM-yy"));
        }

        public static class Customers
        {
            public static Faker<Customer> Valid() => new Faker<Customer>()
                .RuleFor(x => x.Cpf, CPFs.Valid.Generate())
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
                .RuleFor(x => x.Cpf, CPFs.Valid.Generate().ToString())
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
                .RuleFor(x => x.Cpf, CPFs.Valid.Generate().ToString())
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, States.Invalid);

            public static Faker<RegisterCustomerRequest> EmptyName() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.Generate().ToString())
                .RuleFor(x => x.State, States.Valid)
                .RuleFor(x => x.Name, "");

            public static Faker<RegisterCustomerRequest> EmptyState() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.Generate().ToString())
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, "");
        }

        public static class GetCustomerRequests
        {
            public static Faker<GetCustomerRequest> Valid() => new Faker<GetCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid.Generate().ToString());

            public static Faker<GetCustomerRequest> InvalidCpf() => Valid()
                .RuleFor(x => x.Cpf, CPFs.Invalid.ToString());
        }
    }
}
