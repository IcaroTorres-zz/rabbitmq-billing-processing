using Library.Optimizations;
using System;
using ValueOf;

namespace Library.ValueObjects
{
    public class Cpf : ValueOf<ulong, Cpf>
    {
        private const int MinRandomNumberValue = 1000000;
        private const int MaxRandomNumberValue = 900000000;
        private bool _isValid;
        public bool IsValid() => _isValid;
        private static readonly Random _random = new Random(123);
        private static readonly object _syncObj = new object();

        public static Cpf NewCpf()
        {
            Cpf cpf;
            do { cpf = From(GenerateRandom()); }
            while (!cpf.IsValid());
            return cpf;
        }

        public static bool TryParse(string value, out ulong parsedValue)
        {
            ReadOnlySpan<char> cpf = value ?? "0";
            return cpf.TryParseUlong(out parsedValue);
        }

        public static ulong GenerateRandom()
        {
            lock (_syncObj)
            {
                ulong number = (ulong)_random.Next(MinRandomNumberValue, MaxRandomNumberValue);
                var seed = number.ToString("000000000");
                var digit1 = GenerateVerifierDigit1(seed);
                var digit2 = GenerateVerifierDigit2(seed, digit1);
                ulong numberToSumDigits = number * 100;
                ulong digit1AsTens = (ulong)(digit1 * 10);
                ulong numberAndDigit1 = numberToSumDigits + digit1AsTens;
                ulong numberFinishedByDigits = numberAndDigit1 + digit2;
                return numberFinishedByDigits;
            }
        }

        public override string ToString()
        {
            return Value.ToString("00000000000");
        }

        public static bool Validate(ReadOnlySpan<char> value)
        {
            if (!ValidateFormat(value)) return false;
            var seed = value.Slice(0, 9);
            var verifierDigits = value.Slice(9, 2);
            var verifierDigit1 = GenerateVerifierDigit1(seed);
            if (verifierDigits[0] - '0' != verifierDigit1) return false;
            var verifierDigit2 = GenerateVerifierDigit2(seed, verifierDigit1);
            return verifierDigits[1] - '0' == verifierDigit2;
        }

        protected override void Validate()
        {
            _isValid = Validate(Value.ToString("00000000000"));
        }

        private static bool ValidateFormat(ReadOnlySpan<char> value)
        {
            if (value.Length != 11) return false;
            foreach (char c in value) if (!char.IsNumber(c)) return false;
            return true;
        }

        private static byte GenerateVerifierDigit1(ReadOnlySpan<char> seed)
        {
            var multiplier1 = new byte[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = SumIteratedMultipliers(multiplier1, seed);
            return Mod11(sum);
        }

        private static byte GenerateVerifierDigit2(ReadOnlySpan<char> seed, byte verifierDigit1)
        {
            var multiplier2 = new byte[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = SumIteratedMultipliers(multiplier2, seed);
            sum += verifierDigit1 * multiplier2[^1];
            return Mod11(sum);
        }

        private static int SumIteratedMultipliers(byte[] multiplier, ReadOnlySpan<char> seed)
        {
            int sum = 0;
            for (byte i = 0; i < 9; i++)
                sum += (seed[i] - '0') * multiplier[i];
            return sum;
        }

        private static byte Mod11(int sum)
        {
            byte verifierDigit2 = (byte)(sum % 11);
            return (byte)(verifierDigit2 < 2 ? 0 : 11 - verifierDigit2);
        }
    }
}
