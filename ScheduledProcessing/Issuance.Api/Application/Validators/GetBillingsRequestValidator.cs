using FluentValidation;
using Issuance.Api.Application.Models;
using Library.Validators;
using Library.ValueObjects;

namespace Issuance.Api.Application.Validators
{
    public class GetBillingsRequestValidator : AbstractValidator<GetBillingsRequest>
    {
        public GetBillingsRequestValidator(ICpfValidator cpfValidator)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Cpf) || !string.IsNullOrWhiteSpace(x.Month))
                .WithMessage("Informe um valor para ao menos um dos dois Cpf e/ou mês")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Cpf)
                        .SetValidator(cpfValidator)
                        .When(x => !string.IsNullOrWhiteSpace(x.Cpf));

                    RuleFor(x => x.Month)
                        .Must(x => Date.TryParseMonth(x, out _, out _))
                        .WithMessage("Vencimento precisa atender o formato [MM-yyyy], com mês de 1 a 12 e ano >= 2000")
                        .When(x => !string.IsNullOrWhiteSpace(x.Month));
                });
        }
    }
}
