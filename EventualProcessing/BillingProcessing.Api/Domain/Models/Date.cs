using MongoDB.Bson.Serialization.Attributes;

namespace BillingProcessing.Api.Domain.Models
{
    public class Date
    {
        [BsonElement("day")] public byte Day { get; set; }
        [BsonElement("month")] public byte Month { get; set; }
        [BsonElement("year")] public ushort Year { get; set; }
    }
}
