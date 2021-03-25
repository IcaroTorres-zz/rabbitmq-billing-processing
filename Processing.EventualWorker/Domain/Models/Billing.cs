using Library.Results;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Processing.EventualWorker.Domain.States.Billings;
using System;

namespace Processing.EventualWorker.Domain.Models
{
    public class Billing : IRequest<IResult>
    {
        [BsonId]
        public virtual string Id { get; set; }

        [BsonElement("cpf")]
        public virtual ulong Cpf { get; set; }

        [BsonElement("amount"), BsonRepresentation(BsonType.Decimal128)]
        public virtual decimal Amount { get; set; }

        [BsonElement("processed_at"), BsonRepresentation(BsonType.DateTime)]
        public virtual DateTime? ProcessedAt { get; set; }

        internal void BeProcessed(decimal amount, DateTime processedAt)
        {
            BillingState.BeProcessed(amount, processedAt);
        }

        private BillingState _billingState;
        internal BillingState BillingState
        {
            get => _billingState ??= BillingState.NewState(this);
            set { _billingState = value; }
        }
        internal virtual void ChangeState(BillingState newState)
        {
            BillingState = newState;
            Amount = BillingState.Amount;
            ProcessedAt = BillingState.ProcessedAt;
        }
    }
}
