namespace Customers.Api.Domain.Models
{
    public class Customer
    {
        public static NullCustomer Null = new NullCustomer();

        public virtual ulong Cpf { get; internal set; }
        public virtual string Name { get; internal set; }
        public virtual string State { get; internal set; }
    }
}
