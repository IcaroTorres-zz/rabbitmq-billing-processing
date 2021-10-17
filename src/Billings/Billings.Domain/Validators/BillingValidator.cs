using Billings.Domain.Models;
using FluentValidation;
using Library.ValueObjects;

namespace Billings.Domain.Validators
{
    public class BillingValidator : AbstractValidator<Billing>
    {
        public BillingValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Valor não pode ser 0 ou negativo");
            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("Cpf não pode ser vazio ou nulo")
                .Must(x => Cpf.Validate(x.ToString("00000000000"))).WithMessage("Cpf inválido");
            RuleFor(x => x.DueDate.Day)
                .NotEmpty().WithMessage("Dia não pode ser vazio ou nulo")
                .GreaterThan((byte)0).WithMessage("Dia precisa ser >= 0");
            RuleFor(x => x.DueDate.Month)
                .NotEmpty().WithMessage("Dia não pode ser vazio ou nulo")
                .GreaterThan((byte)0).WithMessage("Dia precisa ser >= 0");
            RuleFor(x => x.DueDate.Year)
                .NotEmpty().WithMessage("Dia não pode ser vazio ou nulo")
                .GreaterThan((ushort)0).WithMessage("Dia precisa ser >= 0");
        }
    }
}
