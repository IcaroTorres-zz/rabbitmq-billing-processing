using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using System;

namespace BillingProcessing.Api.Domain.States.Billings
{
    public abstract class BillingState
    {
        protected BillingState(Billing context)
        {
            Context = context;
        }
        internal static BillingState NewState(Billing context)
        {
            return context.ProcessedAt.HasValue
                ? new ProcessedBillingState(context, context.Amount, context.ProcessedAt, context.CustomerState)
                : new PendingBillingState(context) as BillingState;
        }
        protected Billing Context { get; set; }
        internal DateTime? ProcessedAt { get; set; }
        internal decimal Amount { get; set; }
        internal string CustomerState { get; set; }

        internal abstract void BeProcessed(IAmountCalculator calculator, Customer customer);
    }
}
