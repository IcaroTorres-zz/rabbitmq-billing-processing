using Processing.Eventual.Domain.Models;
using System;

namespace Processing.Eventual.Domain.States
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
