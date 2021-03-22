using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;

namespace BillingProcessing.Api.Application.Services
{
    public class MathOnlyAmountCalculator : IAmountCalculator
    {
        private const uint firstTwoDigitIsolationDivider = 1000000000;
        private const ushort firstTwoDigitHundredsMultiplier = 100;
        private const ushort tesnAndUnitsIsolatorMod = 100;

        public decimal Calculate(Customer customer)
        {
            var dozensAndUnits = CalculateTensAndUnits(customer.Cpf);
            var thousandsAndhundreds = CalculateThousandsAndHundreds(customer.Cpf);
            return thousandsAndhundreds + dozensAndUnits;
        }

        private byte CalculateTensAndUnits(ulong cpf)
        {
            var tensAndUnits = cpf % tesnAndUnitsIsolatorMod;
            return (byte)tensAndUnits;
        }

        private ushort CalculateThousandsAndHundreds(ulong cpf)
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
