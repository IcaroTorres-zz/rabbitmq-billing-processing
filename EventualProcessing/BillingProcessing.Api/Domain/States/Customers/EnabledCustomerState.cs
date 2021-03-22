using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;

namespace BillingProcessing.Api.Domain.States.Customers
{
    public class EnabledCustomerState : CustomerState
    {
        public EnabledCustomerState(Customer context) : base(context)
        {
        }

        internal override bool Active { get => true; set { _ = value; } }

        internal override void BeEnabled()
        {
        }
        internal override void BeDisabled()
        {
            Context.ChangeState(new DisabledCustomerState(Context));
        }
        internal override void AcceptProcessing(Billing billing, IAmountCalculator calculator)
        {
            billing.BeProcessedBy(calculator, Context);
        }
    }
}
