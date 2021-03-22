using BillingProcessing.Api.Domain.Models;

namespace BillingProcessing.Api.Application.Abstractions
{
    public interface IAmountCalculator
    {
        decimal Calculate(Customer customer);
    }
}
