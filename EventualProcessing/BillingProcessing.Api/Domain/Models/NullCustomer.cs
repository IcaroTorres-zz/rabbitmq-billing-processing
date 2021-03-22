using BillingProcessing.Api.Application.Abstractions;
using PrivatePackage.Abstractions;

namespace BillingProcessing.Api.Domain.Models
{
    public class NullCustomer : Customer, INull
    {
        public override ulong Cpf { get => 0; set { } }
        public override string State { get => string.Empty; set { } }

        public override void AcceptProcessing(Billing charge, IAmountCalculator processor)
        {
        }
    }
}
