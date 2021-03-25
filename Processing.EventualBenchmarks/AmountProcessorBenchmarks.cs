using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Processing.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AmountProcessorBenchmarks
    {
        private static readonly ScheduledWorker.Domain.Models.Customer _customer =
            new ScheduledWorker.Domain.Models.Customer { Cpf = 92903084530 };
        private static ScheduledWorker.Domain.Models.Billing _billing =
            new ScheduledWorker.Domain.Models.Billing { Cpf = 92903084530 };
        private static readonly ScheduledWorker.Domain.Services.MathOnlyAmountProcessor _mathOnlyProcessor =
            new ScheduledWorker.Domain.Services.MathOnlyAmountProcessor();

        private static readonly EventualWorker.Domain.Models.Customer _customer2 =
            new EventualWorker.Domain.Models.Customer { Cpf = 92903084530 };
        private static EventualWorker.Domain.Models.Billing _billing2 =
            new EventualWorker.Domain.Models.Billing { Cpf = 92903084530 };
        private static readonly EventualWorker.Application.Services.MathOnlyAmountProcessor _mathOnlyProcessor2 =
            new EventualWorker.Application.Services.MathOnlyAmountProcessor();


        [Benchmark]
        public void Scheduled_MathOnlyProcessor_Process()
        {
            _billing = _mathOnlyProcessor.Process(_customer, _billing);
        }

        [Benchmark]
        public void Eventual_MathOnlyProcessor_Process()
        {
            _billing2 = _mathOnlyProcessor2.Process(_customer2, _billing2);
        }
    }
}
