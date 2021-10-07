using Bogus;
using Customers.Application.Requests;
using Customers.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static Library.TestHelpers.Fakes;

namespace UnitTests.Customers.Helpers
{
    public static class InternalFakes
    {
        public static class Customers
        {
            public static Faker<Customer> Valid() => new Faker<Customer>()
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate())
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, States.Valid);
        }

        public static class RegisterCustomerRequests
        {
            public static Faker<RegisterCustomerRequest> Valid() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate().ToString())
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
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate().ToString())
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, States.Invalid);

            public static Faker<RegisterCustomerRequest> EmptyName() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate().ToString())
                .RuleFor(x => x.State, States.Valid)
                .RuleFor(x => x.Name, "");

            public static Faker<RegisterCustomerRequest> EmptyState() => new Faker<RegisterCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate().ToString())
                .RuleFor(x => x.Name, x => x.Name.FullName())
                .RuleFor(x => x.State, "");
        }

        public static class GetCustomerRequests
        {
            public static Faker<GetCustomerRequest> Valid() => new Faker<GetCustomerRequest>()
                .RuleFor(x => x.Cpf, CPFs.Valid().Generate().ToString());

            public static Faker<GetCustomerRequest> InvalidCpf() => Valid()
                .RuleFor(x => x.Cpf, CPFs.Invalid.ToString());
        }
    }
}
