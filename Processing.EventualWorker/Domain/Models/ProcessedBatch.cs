using Library.Results;
using MediatR;
using System.Collections.Generic;

namespace Processing.EventualWorker.Domain.Models
{
    public class ProcessedBatch : List<Billing>, IRequest<IResult>
    {
        public ProcessedBatch(IEnumerable<Billing> items) : base(items) { }
    }
}