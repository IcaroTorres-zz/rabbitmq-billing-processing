using Billings.Application.Models;
using FluentValidation;
using Library.ValueObjects;

namespace Billings.Application.Validators
{
    public class GetBillingsRequestValidator : AbstractValidator<GetBillingsRequest>
    {
        public GetBillingsRequestValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Cpf) || !string.IsNullOrWhiteSpace(x.Month))
                .WithMessage("Informe um valor para ao menos um dos dois Cpf e/ou mês")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Cpf)
                        .NotEmpty().WithMessage("Cpf não pode ser vazio ou nulo")
                        .Must(x => Cpf.Validate(x)).WithMessage("Cpf inválido")
                        .When(x => !string.IsNullOrWhiteSpace(x.Cpf));

                    RuleFor(x => x.Month)
                        .Must(x => Date.TryParseMonth(x, out _, out _))
                        .WithMessage("Vencimento precisa atender o formato [MM-yyyy], com mês de 1 a 12 e ano >= 2000")
                        .When(x => !string.IsNullOrWhiteSpace(x.Month));
                });
        }
    }
}
