using BenchmarkDotNet.Running;

namespace BillingProcessing.Benchmarks
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<AmountCalculatorBenchmarks>();
        }
    }
}
