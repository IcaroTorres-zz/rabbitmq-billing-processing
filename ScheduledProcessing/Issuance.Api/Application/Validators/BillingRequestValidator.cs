using FluentValidation;
using Issuance.Api.Application.Models;
using Library.Optimizations;
using Library.Validators;
using System;

namespace Issuance.Api.Application.Validators
{
    public class BillingRequestValidator : AbstractValidator<BillingRequest>
    {
        public BillingRequestValidator(ICpfValidator cpfValidator)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Valor não pode ser 0 ou negativo");
            RuleFor(x => x.Cpf).SetValidator(cpfValidator);

            RuleFor(x => x.DueDate)
                .Must(x => ValidateFutureDate(x))
                .WithMessage("Vencimento precisa representar uma data válida futura no formato [dd-MM-yyyy]");
        }

        private bool ValidateFutureDate(ReadOnlySpan<char> duedate)
        {
            if (duedate.Length != 10) return false;

            if (duedate.Slice(6, 4).TryParseUshort(out var year) &&
                duedate.Slice(3, 2).TryParseByte(out var month) &&
                duedate.Slice(0, 2).TryParseByte(out var day))
            {
                return (year > DateTime.Today.Year) ||
                       (year == DateTime.Today.Year && month > DateTime.Today.Month) ||
                       (year == DateTime.Today.Year && month == DateTime.Today.Month && day > DateTime.Today.Day);
            }
            return false;
        }
    }
}
