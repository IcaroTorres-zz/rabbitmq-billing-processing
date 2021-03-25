using Library.Messaging;
using Library.Results;
using MediatR;
using Processing.EventualWorker.Application.Abstractions;
using Processing.EventualWorker.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Processing.EventualWorker.Application.Usecases
{
    public class HandleBillingIssuedUsecase : IRequestHandler<Billing, IResult>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBillingsRepository _billingRepository;
        private readonly IAmountProcessor _processor;
        private readonly IMessagePublisher _publisher;

        public HandleBillingIssuedUsecase(
            ICustomerRepository customerRepository,
            IBillingsRepository billingRepository,
            IAmountProcessor processor,
            IMessagePublisher publisher)
        {
            _customerRepository = customerRepository;
            _billingRepository = billingRepository;
            _processor = processor;
            _publisher = publisher;
        }

        public async Task<IResult> Handle(Billing request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetAsync(request.Cpf, cancellationToken);
            var processDone = customer.AcceptProcessing(request, _processor);
            await _billingRepository.InsertAsync(request, cancellationToken);
            if (processDone)
            {
                var processedBatch = new ProcessedBatch(new[] { request });
                await _publisher.Publish(new BasicMessage(processedBatch, nameof(HandleBillingIssuedUsecase)));
            }
            return new SuccessResult(request);
        }
    }
}
