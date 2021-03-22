using BenchmarkDotNet.Running;

namespace BillingProcessing.Benchmarks
{
    public class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<AmountCalculatorBenchmarks>();
        }
    }
}
