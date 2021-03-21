using System.Diagnostics.CodeAnalysis;

namespace Processing.Worker
{
    [ExcludeFromCodeCoverage]
    public class ScheduledProcessorSettings
    {
        public virtual int MillisecondsScheduledTime { get; set; }
    }
}
