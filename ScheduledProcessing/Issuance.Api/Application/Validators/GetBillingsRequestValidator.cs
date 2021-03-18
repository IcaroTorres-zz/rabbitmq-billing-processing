using FluentValidation;
using Issuance.Api.Application.Models;
using Library.Abstractions;
using Library.Optimizations;
using System;

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
                        .Must(x => ValidateMonth(x))
                        .WithMessage("Vencimento precisa atender o formato [MM-yyyy], com mês de 1 a 12 e ano >= 2000")
                        .When(x => !string.IsNullOrWhiteSpace(x.Month));
                });
        }

        private bool ValidateMonth(ReadOnlySpan<char> monthYear)
        {
            if (monthYear.Length != 7) return false;
            if (!monthYear.Slice(0, 2).TryParseByte(out var parsedMonth) || parsedMonth < 1 || parsedMonth > 12) return false;
            return monthYear.Slice(3, 4).TryParseUshort(out var parsedYear) && parsedYear >= 2000;
        }
    }
}
