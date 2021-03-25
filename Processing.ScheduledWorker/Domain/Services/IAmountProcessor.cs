using Processing.ScheduledWorker.Domain.Models;

namespace Processing.ScheduledWorker.Domain.Services
{
    public interface IAmountProcessor
    {
        Billing Process(ICpfCarrier customer, Billing billing);
    }
}
