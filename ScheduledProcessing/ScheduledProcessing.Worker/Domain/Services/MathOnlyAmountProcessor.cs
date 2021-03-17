using ScheduledProcessing.Worker.Domain.Models;
using System;

namespace ScheduledProcessing.Worker.Domain.Services
{
    public class MathOnlyAmountProcessor : IAmountProcessor
    {
        private const uint firstTwoDigitIsolationDivider = 1000000000;
        private const ushort firstTwoDigitHundredsMultiplier = 100;
        private const ushort tesnAndUnitsIsolatorMod = 100;

        public Billing Process(Customer customer, Billing billing)
        {
            var tensAndUnits = ProcessTensAndUnits(customer.Cpf);
            var thousandsAndhundreds = ProcessThousandsAndHundreds(customer.Cpf);
            billing.Amount = thousandsAndhundreds + tensAndUnits;
            billing.ProcessedAt = DateTime.UtcNow;
            return billing;
        }

        private byte ProcessTensAndUnits(ulong cpf)
        {
            var tensAndUnits = cpf % tesnAndUnitsIsolatorMod;
            return (byte)tensAndUnits;
        }

        private ushort ProcessThousandsAndHundreds(ulong cpf)
        {
            var firstTwoDigit = IsolateFirstTwoDigit(cpf);
            var thousandsAndhundreds = firstTwoDigit * firstTwoDigitHundredsMultiplier;
            return (ushort)thousandsAndhundreds;
        }

        private byte IsolateFirstTwoDigit(ulong cpf)
        {
            var firstTwoDigitWithDecimals = cpf / firstTwoDigitIsolationDivider;
            var firstTwoDigitTruncated = decimal.Truncate(firstTwoDigitWithDecimals);
            return (byte)firstTwoDigitTruncated;
        }
    }
}
