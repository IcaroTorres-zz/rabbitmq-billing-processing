using BillingProcessing.Api.Application.Abstractions;
using BillingProcessing.Api.Domain.States.Customers;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrivatePackage.Abstractions;

namespace BillingProcessing.Api.Domain.Models
{
    public class Customer : IRequest<IResult>
    {
        [BsonId, BsonRepresentation(BsonType.Int64), BsonElement("cpf")]
        public virtual ulong Cpf { get; set; }

        [BsonElement("state")]
        public virtual string State { get; set; }

        [BsonElement("active")]
        public virtual bool Active { get; set; }

        public virtual void EnableProcessing()
        {
            CustomerState.BeEnabled();
        }

        public virtual void DisableProcessing()
        {
            CustomerState.BeDisabled();
        }

        public virtual void AcceptProcessing(Billing charge, IAmountCalculator calculator)
        {
            CustomerState.AcceptProcessing(charge, calculator);
        }

        private CustomerState customerState;
        internal CustomerState CustomerState
        {
            get => customerState ??= CustomerState.NewState(this);
            set { customerState = value; }
        }
        internal virtual void ChangeState(CustomerState newState)
        {
            Active = newState.Active;
            CustomerState = newState;
        }

        public static readonly Customer Null = new NullCustomer();
    }
}
