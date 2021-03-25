using Processing.EventualWorker.Domain.Models;
using System;

namespace Processing.EventualWorker.Domain.States.Billings
{
    public class PendingBillingState : BillingState
    {
        internal PendingBillingState(Billing context) : base(context)
        {
        }
        internal override void BeProcessed(decimal amount, DateTime processedAt)
        {
            Context.ChangeState(new ProcessedBillingState(Context, amount, processedAt));
        }
    }
}
