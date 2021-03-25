namespace Processing.ScheduledWorker.Domain.Models
{
    public class Customer : ICpfCarrier
    {
        public ulong Cpf { get; set; }
    }
}
