using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BillingProcessing.Api.Application.Services;
using BillingProcessing.Api.Domain.Models;

namespace BillingProcessing.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AmountCalculatorBenchmarks
    {
        private static readonly Customer Customer = new Customer { Cpf = 92903084530 };
        private static readonly MathOnlyAmountCalculator mathOnlyCalculator = new MathOnlyAmountCalculator();
        private static readonly SpanAndMathAmountCalculator spanAndMathCalculator = new SpanAndMathAmountCalculator();
        private static readonly StringBuilderAndSpanAmountCalculator stringBuilderAndSpanCalculator = new StringBuilderAndSpanAmountCalculator();
        private static readonly ToStringAmountCalculator toStringAmountCalculator = new ToStringAmountCalculator();

        [Benchmark]
        public void MathOnlyCalclator_Calculate()
        {
            mathOnlyCalculator.Calculate(Customer);
        }

        [Benchmark]
        public void SpanAndMathCalculator_Calculate()
        {
            spanAndMathCalculator.Calculate(Customer);
        }

        [Benchmark]
        public void StringBuilderAndSpanCalculator_Calculate()
        {
            stringBuilderAndSpanCalculator.Calculate(Customer);
        }

        [Benchmark]
        public void ToStringAmountCalculator_Calculate()
        {
            toStringAmountCalculator.Calculate(Customer);
        }
    }
}
