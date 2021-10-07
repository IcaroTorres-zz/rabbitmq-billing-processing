using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Domain.Services
{
    public interface IAmountProcessor
    {
        Billing Process(Customer customer, Billing billing);
    }
}
