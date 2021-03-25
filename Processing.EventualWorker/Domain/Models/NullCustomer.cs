using Library.Results;
using Processing.EventualWorker.Application.Abstractions;

namespace Processing.EventualWorker.Domain.Models
{
    public class NullCustomer : Customer, INull
    {
        public override ulong Cpf { get => 0; set { _ = value; } }
        public override bool AcceptProcessing(Billing charge, IAmountProcessor calculator)
        {
            return false;
        }
    }
}
