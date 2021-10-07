using Billings.Domain.Models;

namespace Billings.Domain.Services
{
    /// <summary>
    /// Implementation for creating Domain Models
    /// </summary>
    public interface IModelFactory
    {
        Billing CreateBilling(string cpfString, double amount, string dueDate);
    }
}
