using FluentValidation;
using Library.ValueObjects;

namespace Library.Validators
{
    public class CpfValidator : AbstractValidator<string>, ICpfValidator
    {
        public CpfValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty().WithMessage("Cpf não pode ser vazio ou nulo")
                .Must(x => Cpf.Validate(x)).WithMessage("Cpf inválido");
        }
    }
}
