using Library.Abstractions;

namespace Customers.Api.Domain.Models
{
    public class NullCustomer : Customer, INull
    {
        public override ulong Cpf { get => 0; internal set { } }
        public override string Name { get => string.Empty; internal set { } }
        public override string State { get => string.Empty; internal set { } }
    }
}
