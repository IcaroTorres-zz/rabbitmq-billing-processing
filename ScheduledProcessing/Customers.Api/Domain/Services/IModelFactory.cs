using Customers.Api.Domain.Models;

namespace Customers.Api.Domain.Services
{
    /// <summary>
    /// Implementation for creating Domain Models
    /// </summary>
    public interface IModelFactory
    {
        /// <summary>
        /// Creates a new complete <see cref="Customer"/> model
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        Customer CreateCustomer(string cpf, string name, string state);
    }
}
