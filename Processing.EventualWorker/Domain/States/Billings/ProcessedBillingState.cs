using Processing.EventualWorker.Domain.Models;
using System;

namespace Processing.EventualWorker.Domain.States.Billings
{
    public class ProcessedBillingState : BillingState
    {
        internal ProcessedBillingState(Billing context, decimal amount, DateTime? processedAt) : base(context)
        {
            Amount = amount;
            ProcessedAt = processedAt;
        }

        internal override void BeProcessed(decimal amount, DateTime processedAt)
        {
            // skiping real implementation not applied for this state
        }
    }
}
