using ScheduledProcessing.Worker.Domain.Models;
using System;

namespace ScheduledProcessing.Worker.Domain.Services
{
    public class MathOnlyAmountProcessor : IAmountProcessor
    {
        private const uint FirstTwoDigitIsolationDivider = 1000000000;
        private const ushort FirstTwoDigitHundredsMultiplier = 100;
        private const ushort TesnAndUnitsIsolatorMod = 100;

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
            var tensAndUnits = cpf % TesnAndUnitsIsolatorMod;
            return (byte)tensAndUnits;
        }

        private ushort ProcessThousandsAndHundreds(ulong cpf)
        {
            var firstTwoDigit = IsolateFirstTwoDigit(cpf);
            var thousandsAndhundreds = firstTwoDigit * FirstTwoDigitHundredsMultiplier;
            return (ushort)thousandsAndhundreds;
        }

        private byte IsolateFirstTwoDigit(ulong cpf)
        {
            var firstTwoDigitWithDecimals = cpf / FirstTwoDigitIsolationDivider;
            var firstTwoDigitTruncated = decimal.Truncate(firstTwoDigitWithDecimals);
            return (byte)firstTwoDigitTruncated;
        }
    }
}
