using Processing.Worker.Domain.Models;

namespace Processing.Worker.Domain.Services
{
    public interface IAmountProcessor
    {
        Billing Process(Customer customer, Billing billing);
    }
}
