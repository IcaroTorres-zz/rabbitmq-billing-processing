using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Application.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using PrivatePackage.Abstractions;

namespace BillingProcessing.Api.Application.Validators
{
    public class DisableCustomerRequestValidator : AbstractValidator<DisableCustomerRequest>
    {
        public DisableCustomerRequestValidator(ICpfValidator cpfValidator, ICustomerRepository customerRepository)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x)
                .ChildRules(c => c.RuleFor(x => x.CpfString).SetValidator(cpfValidator))
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.CpfLong)
                        .MustAsync((x, ct) => customerRepository.ExistEnabledAsync(x, ct))
                        .WithMessage(x => $"Cliente com Cpf {x.CpfLong} já está desabilitado")
                        .WithErrorCode(StatusCodes.Status404NotFound.ToString());
                });
        }
    }
}
