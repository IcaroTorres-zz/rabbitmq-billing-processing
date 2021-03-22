using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;

namespace BillingProcessing.Api.Domain.States.Customers
{
    public abstract class CustomerState
    {
        protected CustomerState(Customer context)
        {
            Context = context;
        }
        internal static CustomerState NewState(Customer customer)
        {
            return customer.Active
                ? new EnabledCustomerState(customer)
                : new DisabledCustomerState(customer) as CustomerState;
        }

        protected Customer Context { get; set; }
        internal virtual bool Active { get; set; }

        internal abstract void BeEnabled();
        internal abstract void BeDisabled();
        internal abstract void AcceptProcessing(Billing billing, IAmountCalculator calculator);
    }
}
