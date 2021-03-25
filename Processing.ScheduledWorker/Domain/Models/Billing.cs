﻿using System;

namespace Processing.ScheduledWorker.Domain.Models
{
    public class Billing : ICpfCarrier
    {
        public Guid Id { get; set; }
        public ulong Cpf { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}