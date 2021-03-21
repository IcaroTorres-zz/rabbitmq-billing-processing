using Library.Results;

namespace Customers.Api.Domain.Models
{
    /// <summary>
    /// Domain representation of a Null Object for business Customer actor
    /// </summary>
    public class NullCustomer : Customer, INull
    {
        /// <inheritdoc cref="Customer.Cpf"/>
        public override ulong Cpf { get => 0; set { Cpf = value; } }
        public override string Name { get => string.Empty; set { Name = value; } }
        public override string State { get => string.Empty; set { State = value; } }
    }
}
