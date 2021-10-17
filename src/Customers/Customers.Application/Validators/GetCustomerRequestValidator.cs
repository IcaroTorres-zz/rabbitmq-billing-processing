using Customers.Application.Abstractions;
using Customers.Application.Requests;
using FluentValidation;
using Library.Optimizations;
using Library.ValueObjects;
using Microsoft.AspNetCore.Http;
using System;

namespace Customers.Application.Validators
{
    public class GetCustomerRequestValidator : AbstractValidator<GetCustomerRequest>
    {
        public GetCustomerRequestValidator(ICustomerRepository repository)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("Cpf não pode ser vazio ou nulo")
                .Must(x => Cpf.Validate(x)).WithMessage("Cpf inválido")
                .MustAsync((cpf, ct) => repository.ExistAsync(cpf.AsSpan().ParseUlong(), ct))
                .WithMessage(x => $"Cliente não encontrado para dado Cpf {x.Cpf}")
                .WithErrorCode(StatusCodes.Status404NotFound.ToString());
        }
    }
}
