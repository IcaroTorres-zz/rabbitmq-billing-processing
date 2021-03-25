using Processing.EventualWorker.Domain.Models;

namespace Processing.EventualWorker.Application.Abstractions
{
    public interface IAmountProcessor
    {
        Billing Process(Customer customer, Billing billing);
    }
}
