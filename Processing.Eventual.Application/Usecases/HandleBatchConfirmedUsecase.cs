using Library.Results;
using MediatR;
using Processing.Eventual.Application.Abstractions;
using Processing.Eventual.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.Eventual.Application.Usecases
{
    public class HandleBatchConfirmedUsecase : IRequestHandler<ProcessedBatch, IResult>
    {
        private readonly IBillingsRepository _repository;

        public HandleBatchConfirmedUsecase(IBillingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IResult> Handle(ProcessedBatch request, CancellationToken cancellationToken)
        {
            await _repository.RemoveManyConfirmedAsync(request, cancellationToken);
            return new SuccessResult(request);
        }
    }
}
