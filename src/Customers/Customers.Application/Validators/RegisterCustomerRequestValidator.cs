using Customers.Application.Abstractions;
using Customers.Application.Requests;
using Customers.Domain.Models;
using Customers.Domain.Services;
using FluentValidation;
using Library.Optimizations;
using Microsoft.AspNetCore.Http;
using System;

namespace Customers.Application.Validators
{
    public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
    {
        public RegisterCustomerRequestValidator(IModelFactory factory, IValidator<Customer> validator, ICustomerRepository repository)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => factory.CreateCustomer(x.Cpf, x.Name, x.State)).SetValidator(validator);
            RuleFor(x => x.Cpf)
                .MustAsync(async (cpf, ct) => !await repository.ExistAsync(cpf.AsSpan().ParseUlong(), ct))
                .WithMessage(x => $"Cliente já existe para dado Cpf {x.Cpf}")
                .WithErrorCode(StatusCodes.Status409Conflict.ToString());
        }
    }
}
