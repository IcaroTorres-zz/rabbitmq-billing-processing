using BenchmarkDotNet.Running;

namespace Processing.Benchmarks
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<AmountProcessorBenchmarks>();
        }
    }
}
