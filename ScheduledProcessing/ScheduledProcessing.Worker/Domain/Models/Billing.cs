using System;

namespace ScheduledProcessing.Worker.Domain.Models
{
    public class Billing
    {
        public Guid Id { get; set; }
        public ulong Cpf { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
