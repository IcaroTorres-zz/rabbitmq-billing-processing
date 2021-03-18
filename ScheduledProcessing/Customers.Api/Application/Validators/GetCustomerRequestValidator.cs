using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using FluentValidation;
using Library.Abstractions;
using Library.Optimizations;
using Microsoft.AspNetCore.Http;
using System;

namespace Customers.Api.Application.Validators
{
    public class GetCustomerRequestValidator : AbstractValidator<GetCustomerRequest>
    {
        public GetCustomerRequestValidator(ICpfValidator cpfValidator, ICustomerRepository repository)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Cpf)
                .SetValidator(cpfValidator)
                .MustAsync((x, ct) => repository.ExistAsync(x.AsSpan().ParseUlong(), ct))
                .WithMessage(x => $"Cliente não encontrado para dado Cpf {x.Cpf}")
                .WithErrorCode(StatusCodes.Status404NotFound.ToString());
        }
    }
}
