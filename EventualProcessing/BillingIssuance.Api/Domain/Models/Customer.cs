using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using PrivatePackage.Abstractions;

namespace BillingIssuance.Api.Domain.Models
{
    public class Customer : IRequest<IResult>
    {
        [BsonId]
        public ulong Cpf { get; set; }

        [BsonElement("active")]
        public bool Active { get; set; }

        public virtual void EnableCharges()
        {
            Active = true;
        }
    }
}
