using Library.Messaging;
using Library.Results;
using MediatR;
using Processing.EventualWorker.Application.Abstractions;
using Processing.EventualWorker.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.EventualWorker.Application.Usecases
{
    public class HandleCustomerRegisteredUsecase : IRequestHandler<Customer, IResult>
    {
        private readonly IBillingsRepository _billingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAmountProcessor _processor;
        private readonly IMessagePublisher _publisher;

        public HandleCustomerRegisteredUsecase(
            IBillingsRepository billingRepository,
            ICustomerRepository customerRepository,
            IAmountProcessor calculator,
            IMessagePublisher publisher)
        {
            _billingRepository = billingRepository;
            _customerRepository = customerRepository;
            _processor = calculator;
            _publisher = publisher;
        }

        public async Task<IResult> Handle(Customer request, CancellationToken cancellationToken)
        {
            var pendingBillingsTask = _billingRepository.GetCustomerPendingBillingsAsync(request.Cpf, cancellationToken);
            await _customerRepository.InsertAsync(request, cancellationToken);
            var pendingBillings = await pendingBillingsTask;
            await Task.Run(() =>
            {
                Parallel.ForEach(pendingBillings, billing => request.AcceptProcessing(billing, _processor));
            }, cancellationToken);
            var processedBatch = new ProcessedBatch(pendingBillings);
            await _billingRepository.UpdateManyProcessedAsync(processedBatch, cancellationToken);
            await _publisher.Publish(new BasicMessage(processedBatch, nameof(HandleCustomerRegisteredUsecase)));
            return new SuccessResult(request);
        }
    }
}
