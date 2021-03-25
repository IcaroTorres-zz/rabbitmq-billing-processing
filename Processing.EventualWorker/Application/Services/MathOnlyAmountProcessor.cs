using Processing.EventualWorker.Application.Abstractions;
using Processing.EventualWorker.Domain.Models;
using System;

namespace Processing.EventualWorker.Application.Services
{
    public class MathOnlyAmountProcessor : IAmountProcessor
    {
        private const uint _firstTwoDigitIsolationDivider = 1000000000;
        private const ushort _firstTwoDigitHundredsMultiplier = 100;
        private const ushort _tesnAndUnitsIsolatorMod = 100;

        public Billing Process(Customer customer, Billing billing)
        {
            var tensAndUnits = CalculateTensAndUnits(customer.Cpf);
            var thousandsAndhundreds = CalculateThousandsAndHundreds(customer.Cpf);
            billing.BeProcessed(thousandsAndhundreds + tensAndUnits, DateTime.UtcNow);
            return billing;
        }

        private byte CalculateTensAndUnits(ulong cpf)
        {
            var tensAndUnits = cpf % _tesnAndUnitsIsolatorMod;
            return (byte)tensAndUnits;
        }

        private ushort CalculateThousandsAndHundreds(ulong cpf)
        {
            var firstTwoDigit = IsolateFirstTwoDigit(cpf);
            var thousandsAndhundreds = firstTwoDigit * _firstTwoDigitHundredsMultiplier;
            return (ushort)thousandsAndhundreds;
        }

        private byte IsolateFirstTwoDigit(ulong cpf)
        {
            var firstTwoDigitWithDecimals = cpf / _firstTwoDigitIsolationDivider;
            var firstTwoDigitTruncated = decimal.Truncate(firstTwoDigitWithDecimals);
            return (byte)firstTwoDigitTruncated;
        }
    }
}
