using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Library.Abstractions;
using System;

namespace Issuance.Api.Domain.Models
{
    public class Billing : IRequest<IResult>
    {
        [BsonId, BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("cpf")]
        public ulong Cpf { get; set; }

        [BsonElement("amount"), BsonRepresentation(BsonType.Double)]
        public double Amount { get; set; }

        [BsonElement("due_date")]
        public Date DueDate { get; set; }

        [BsonElement("processed_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? ProcessedAt { get; set; }
    }
}
