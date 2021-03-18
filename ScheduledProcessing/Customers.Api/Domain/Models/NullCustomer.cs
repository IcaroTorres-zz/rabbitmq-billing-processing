using Library.Results;

namespace Customers.Api.Domain.Models
{
    /// <summary>
    /// Domain representation of a Null Object for business Customer actor
    /// </summary>
    public class NullCustomer : Customer, INull
    {
        /// <inheritdoc cref="Customer.Cpf"/>
        public override ulong Cpf { get => 0; set { } }
        public override string Name { get => string.Empty; set { } }
        public override string State { get => string.Empty; set { } }
    }
}
