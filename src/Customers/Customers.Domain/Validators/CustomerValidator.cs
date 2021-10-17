using Customers.Domain.Models;
using FluentValidation;
using Library.ValueObjects;

namespace Customers.Domain.Validators
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nome não pode ser vazio ou nulo");
            RuleFor(x => x.State)
                .NotEmpty().WithMessage("Estado não pode ser vazio ou nulo")
                .Must(x => BrazilianState.Validate(x)).WithMessage("Estado inválido");
            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("Cpf não pode ser vazio ou nulo")
                .Must(x => Cpf.Validate(x.ToString("00000000000"))).WithMessage("Cpf inválido");
        }
    }
}
