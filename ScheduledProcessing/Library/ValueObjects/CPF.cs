﻿using Library.Optimizations;
using System;
using ValueOf;

namespace Library.ValueObjects
{
    public class CPF : ValueOf<ulong, CPF>
    {
        private bool _isValid;
        public bool IsValid() => _isValid;

        public static CPF NewCPF()
        {
            var value = GenerateRandom();
            return From(value);
        }

        public static bool TryParse(string? value, out ulong parsedValue)
        {
            ReadOnlySpan<char> cpf = value ?? "0";
            return cpf.TryParseUlong(out parsedValue);
        }

        public static ulong GenerateRandom()
        {
            var rnd = new Random();
            var randomNumber = rnd.Next(100000000, 999999999);
            var seed = randomNumber.ToString();
            var verifierDigit1 = GenerateVerifierDigit1(seed);
            var verifierDigit2 = GenerateVerifierDigit2(seed, verifierDigit1);
            return (ulong)(randomNumber + (verifierDigit1 * 10) + verifierDigit2);
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
            int[] multiplier1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = SumIteratedMultipliers(multiplier1, seed);
            return Mod11(sum);
        }

        private static byte GenerateVerifierDigit2(ReadOnlySpan<char> seed, byte verifierDigit1)
        {
            int[] multiplier2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = SumIteratedMultipliers(multiplier2, seed);
            sum += verifierDigit1 * multiplier2[^1];
            return Mod11(sum);
        }

        private static int SumIteratedMultipliers(int[] multiplier1, ReadOnlySpan<char> seed)
        {
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (seed[i] - '0') * multiplier1[i];
            return sum;
        }

        private static byte Mod11(int sum)
        {
            byte verifierDigit2 = (byte)(sum % 11);
            return (byte)(verifierDigit2 < 2 ? 0 : 11 - verifierDigit2);
        }
    }
}
