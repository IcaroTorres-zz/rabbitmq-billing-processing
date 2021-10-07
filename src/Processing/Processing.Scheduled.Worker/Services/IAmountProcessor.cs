using Processing.Scheduled.Worker.Models;

namespace Processing.Scheduled.Worker.Services
{
    public interface IAmountProcessor
    {
        Billing Process(ICpfCarrier customer, Billing billing);
    }
}
