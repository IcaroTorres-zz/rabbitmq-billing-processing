using Library.Results;
using Processing.Eventual.Domain.Services;

namespace Processing.Eventual.Domain.Models
{
    public class NullCustomer : Customer, INull
    {
        public override ulong Cpf { get => 0; set { _ = value; } }
        public override bool AcceptProcessing(Billing billing, IAmountProcessor calculator)
        {
            return false;
        }
    }
}
