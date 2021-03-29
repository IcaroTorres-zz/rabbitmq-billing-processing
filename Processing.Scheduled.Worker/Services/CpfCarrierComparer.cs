using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Processing.Scheduled.Worker.Services
{
    public class CpfCarrierComparer : IComparer<ICpfCarrier>
    {
        public int Compare([AllowNull] ICpfCarrier x, [AllowNull] ICpfCarrier y) => x.Cpf.CompareTo(y.Cpf);
    }
}
