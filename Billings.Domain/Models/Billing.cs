using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Billings.Domain.Models
{
    /// <summary>
    /// Domain representation of a business Billing charged for a customer
    /// </summary>
    public class Billing
    {
        [BsonId, BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        /// <summary>
        /// Unique personal identification in force in Brazil
        /// </summary>
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
