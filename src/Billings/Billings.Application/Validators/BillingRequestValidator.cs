using Billings.Application.Models;
using FluentValidation;
using Library.Validators;
using Library.ValueObjects;

namespace Billings.Application.Validators
{
    public class BillingRequestValidator : AbstractValidator<BillingRequest>
    {
        public BillingRequestValidator(ICpfValidator cpfValidator)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Valor não pode ser 0 ou negativo");
            RuleFor(x => x.Cpf).SetValidator(cpfValidator);

            RuleFor(x => x.DueDate)
                .Must(x => Date.ValidateFutureDate(x))
                .WithMessage("Vencimento precisa representar uma data válida futura no formato [dd-MM-yyyy]");
        }
    }
}
