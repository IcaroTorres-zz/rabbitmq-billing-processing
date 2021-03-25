using Processing.EventualWorker.Domain.Models;
using System;

namespace Processing.EventualWorker.Domain.States.Billings
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
                ? new ProcessedBillingState(context, context.Amount, context.ProcessedAt)
                : new PendingBillingState(context) as BillingState;
        }
        protected Billing Context { get; set; }
        internal DateTime? ProcessedAt { get; set; }
        internal decimal Amount { get; set; }

        internal abstract void BeProcessed(decimal amount, DateTime processedAt);
    }
}
