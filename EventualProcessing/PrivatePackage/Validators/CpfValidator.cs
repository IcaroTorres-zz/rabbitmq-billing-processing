using FluentValidation;
using PrivatePackage.Abstractions;
using PrivatePackage.ValueObjects;

namespace PrivatePackage.Validators
{
    public class CpfValidator : AbstractValidator<string>, ICpfValidator
    {
        public CpfValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty().WithMessage("Cpf não pode ser vazio ou nulo")
                .Must(x => Cpf.From(x).IsValid).WithMessage("Cpf inválido");
        }
    }
}
