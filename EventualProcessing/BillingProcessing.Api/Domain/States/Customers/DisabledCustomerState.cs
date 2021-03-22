using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;

namespace BillingProcessing.Api.Domain.States.Customers
{
    public class DisabledCustomerState : CustomerState
    {
        public DisabledCustomerState(Customer context) : base(context)
        {
        }

        internal override bool Active { get => false; set { _ = value; } }

        internal override void BeEnabled()
        {
            Context.ChangeState(new EnabledCustomerState(Context));
        }
        internal override void BeDisabled()
        {
        }
        internal override void AcceptProcessing(Billing billing, IAmountCalculator calculator)
        {
        }
    }
}
