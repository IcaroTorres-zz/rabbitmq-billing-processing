using ScheduledProcessing.Worker.Domain.Models;

namespace ScheduledProcessing.Worker.Domain.Services
{
    public interface IAmountProcessor
    {
        Billing Process(Customer customer, Billing billing);
    }
}
