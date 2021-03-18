using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using FluentValidation;
using Library.Abstractions;
using Library.Optimizations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Customers.Api.Application.Validators
{
    public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
    {
        public RegisterCustomerRequestValidator(ICpfValidator cpfValidator, ICustomerRepository repository)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Nome não pode ser vazio ou nulo");
            RuleFor(x => x.State)
                .NotEmpty().WithMessage("Estado não pode ser vazio ou nulo")
                .Must(x => states.Contains(x)).WithMessage("Estado inválido");

            RuleFor(x => x.Cpf)
                .SetValidator(cpfValidator)
                .MustAsync(async (x, ct) => !await repository.ExistAsync(x.AsSpan().ParseUlong(), ct))
                .WithMessage(x => $"Cliente já existe para dado Cpf {x.Cpf}")
                .WithErrorCode(StatusCodes.Status409Conflict.ToString());
        }

        private readonly HashSet<string> states = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
            "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
            "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };
    }
}
