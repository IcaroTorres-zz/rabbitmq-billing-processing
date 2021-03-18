using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Issuance.Api.Domain.Models
{
    /// <summary>
    /// Date representation without the timespan ignored on this domain
    /// </summary>
    public class Date
    {
        [BsonElement("day"), BsonRepresentation(BsonType.Int32)]
        public byte Day { get; set; }

        [BsonElement("month"), BsonRepresentation(BsonType.Int32)]
        public byte Month { get; set; }

        [BsonElement("year"), BsonRepresentation(BsonType.Int32)]
        public ushort Year { get; set; }
    }
}
