using ScheduledProcessing.Worker.Domain.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ScheduledProcessing.Worker.Domain.Services
{
    public class CustomerCpfComparer : IComparer<Customer>
    {
        public int Compare([AllowNull] Customer x, [AllowNull] Customer y) => x.Cpf.CompareTo(y.Cpf);
    }
}
