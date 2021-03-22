using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using System;

namespace BillingProcessing.Api.Domain.States.Billings
{
    public class PendingBillingState : BillingState
    {
        internal PendingBillingState(Billing context) : base(context)
        {
        }
        internal override void BeProcessed(IAmountCalculator calculator, Customer customer)
        {
            var amount = calculator.Calculate(customer);
            Context.ChangeState(new ProcessedBillingState(Context, amount, DateTime.UtcNow, customer.State));
        }
    }
}
