using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.States.Billings;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrivatePackage.Abstractions;
using System;

namespace BillingProcessing.Api.Domain.Models
{
    public class Billing : IRequest<IResult>
    {
        [BsonId]
        public virtual string Id { get; set; }

        [BsonElement("cpf")]
        public virtual ulong Cpf { get; set; }

        [BsonElement("customer_state")]
        public virtual string CustomerState { get; set; }

        [BsonElement("amount"), BsonRepresentation(BsonType.Decimal128)]
        public virtual decimal Amount { get; set; }

        [BsonElement("processed_at"), BsonRepresentation(BsonType.DateTime)]
        public virtual DateTime? ProcessedAt { get; set; }

        private Date dueDate;
        [BsonElement("due_date")]
        public virtual Date DueDate
        {
            get => dueDate;
            set
            {
                if (value != dueDate)
                {
                    dueDateTime = new DateTime(value.Year, value.Month, value.Day);
                    dueDate = value;
                }
            }
        }

        private DateTime dueDateTime;
        [BsonElement("due_date_time")]
        public DateTime DueDateTime => dueDateTime;

        internal void BeProcessedBy(IAmountCalculator calculator, Customer customer)
        {
            BillingState.BeProcessed(calculator, customer);
        }

        private BillingState billingState;
        internal BillingState BillingState
        {
            get => billingState ??= BillingState.NewState(this);
            set { billingState = value; }
        }
        internal virtual void ChangeState(BillingState newState)
        {
            BillingState = newState;
            Amount = BillingState.Amount;
            ProcessedAt = BillingState.ProcessedAt;
            CustomerState = BillingState.CustomerState;
        }
    }
}
