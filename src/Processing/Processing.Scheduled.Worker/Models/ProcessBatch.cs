using Processing.Scheduled.Worker.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processing.Scheduled.Worker.Models
{
    public class ProcessBatch
    {
        public ProcessBatch()
        {
            Id = Guid.NewGuid().ToString();
            Billings = new List<Billing>();
            Customers = new List<ICpfCarrier>();
        }
        public string Id { get; set; }
        public List<Billing> Billings { get; set; }
        public List<ICpfCarrier> Customers { get; set; }
        public async Task<ProcessBatch> ResetIdAfter(int millisecondsScheduledTime)
        {
            var delayTask = Task.Delay(millisecondsScheduledTime);
            Id = Guid.NewGuid().ToString();
            await delayTask;
            return this;
        }
    }
}
