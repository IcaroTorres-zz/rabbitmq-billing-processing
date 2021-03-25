using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Processing.Worker.Application.Services;
using Processing.Worker.Domain.Models;

namespace Processing.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AmountProcessorBenchmarks
    {
        private static readonly Customer _customer = new Customer { Cpf = 92903084530 };
        private static Billing _billing = new Billing { Cpf = 92903084530 };
        private static readonly MathOnlyAmountProcessor _mathOnlyProcessor = new MathOnlyAmountProcessor();

        [Benchmark]
        public void MathOnlyProcessor_Process()
        {
            _billing = _mathOnlyProcessor.Process(_customer, _billing);
        }
    }
}
