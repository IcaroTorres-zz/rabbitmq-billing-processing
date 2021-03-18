using FluentValidation;
using System;

namespace Library.Validators
{
    public class CpfValidator : AbstractValidator<string>, ICpfValidator
    {
        public CpfValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty().WithMessage("Cpf não pode ser vazio ou nulo")
                .Must(x => ValidateFormat(x) && ValidateDigits(x)).WithMessage("Cpf inválido");
        }

        private bool ValidateFormat(ReadOnlySpan<char> value)
        {
            if (value.Length != 11) return false;
            foreach (char c in value) if (!char.IsNumber(c)) return false;
            return true;
        }

        private bool ValidateDigits(ReadOnlySpan<char> value)
        {
            var firstMultipliers = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var secondMultipliers = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var valueDigits = value.Slice(0, 9);
            var verifierDigits = value.Slice(9, 2);

            var sum1 = 0;
            var sum2 = 0;

            for (var i = 0; i < valueDigits.Length; i++)
            {
                var positionValue = valueDigits[i] - '0';
                sum1 += positionValue * firstMultipliers[i];
                sum2 += positionValue * secondMultipliers[i];
            }
            var verifierDigit1 = sum1 % 11;
            verifierDigit1 = verifierDigit1 < 2 ? 0 : 11 - verifierDigit1;

            if (verifierDigits[0] - '0' != verifierDigit1) return false;

            sum2 += verifierDigit1 * secondMultipliers[^1];

            var verifierDigit2 = sum2 % 11;
            verifierDigit2 = verifierDigit2 < 2 ? 0 : 11 - verifierDigit2;

            return verifierDigits[1] - '0' == verifierDigit2;
        }
    }
}
