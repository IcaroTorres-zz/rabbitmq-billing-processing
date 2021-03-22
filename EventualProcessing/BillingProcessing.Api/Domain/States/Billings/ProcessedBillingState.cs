using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.Models;
using System;

namespace BillingProcessing.Api.Domain.States.Billings
{
    public class ProcessedBillingState : BillingState
    {
        internal ProcessedBillingState(Billing context, decimal amount, DateTime? processedAt, string customerState) : base(context)
        {
            Amount = amount;
            ProcessedAt = processedAt;
            CustomerState = customerState;
        }

        internal override void BeProcessed(IAmountCalculator calculator, Customer customer)
        {
        }
    }
}
