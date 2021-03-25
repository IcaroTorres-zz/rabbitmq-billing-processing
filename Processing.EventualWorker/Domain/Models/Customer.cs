using Library.Results;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Processing.EventualWorker.Application.Abstractions;

namespace Processing.EventualWorker.Domain.Models
{
    public class Customer : IRequest<IResult>
    {
        [BsonId, BsonRepresentation(BsonType.Int64), BsonElement("cpf")]
        public virtual ulong Cpf { get; set; }

        public virtual bool AcceptProcessing(Billing billing, IAmountProcessor calculator)
        {
            billing = calculator.Process(this, billing);
            return billing.ProcessedAt != null;
        }

        public static readonly Customer Null = new NullCustomer();
    }
}
