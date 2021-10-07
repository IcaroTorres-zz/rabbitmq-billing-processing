using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Billings.Domain.Models
{
    /// <summary>
    /// Date representation without the timespan ignored on this domain
    /// </summary>
    public class Date
    {
        public Date() { }
        public Date(string value) : this()
        {
            Library.ValueObjects.Date.TryParse(value, out var day, out var month, out var year);
            Day = day;
            Month = month;
            Year = year;
        }

        [BsonElement("day"), BsonRepresentation(BsonType.Int32)]
        public byte Day { get; set; }

        [BsonElement("month"), BsonRepresentation(BsonType.Int32)]
        public byte Month { get; set; }

        [BsonElement("year"), BsonRepresentation(BsonType.Int32)]
        public ushort Year { get; set; }

        public override string ToString()
        {
            return $"{Day:00}-{Month:00}-{Year:0000}";
        }
    }
}
