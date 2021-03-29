using Processing.Scheduled.Worker.Services;

namespace Processing.Scheduled.Worker.Models
{
    public class Customer : ICpfCarrier
    {
        public ulong Cpf { get; set; }
    }
}
