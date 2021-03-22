using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using PrivatePackage.Abstractions;
using PrivatePackage.Optmizations;
using System;

namespace BillingProcessing.Api.Application.Validators
{
    public class CustomerReportRequestValidator : AbstractValidator<CustomerReportRequest>
    {
        public CustomerReportRequestValidator(ICpfValidator cpfValidator, ICustomerRepository customerRepository)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Cpf)
                .SetValidator(cpfValidator)
                .MustAsync((x, ct) => customerRepository.ExistEnabledAsync(x.AsSpan().ParseUlong(), ct))
                .WithMessage(x => $"Cliente não encontrado para dado Cpf {x.Cpf}")
                .WithErrorCode(StatusCodes.Status404NotFound.ToString());
        }
    }
}
