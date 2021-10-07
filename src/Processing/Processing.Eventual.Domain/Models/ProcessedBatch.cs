using Library.Results;
using MediatR;
using System.Collections.Generic;

namespace Processing.Eventual.Domain.Models
{
    public class ProcessedBatch : List<Billing>, IRequest<IResult>
    {
        public ProcessedBatch(IEnumerable<Billing> items) : base(items) { }
    }
}