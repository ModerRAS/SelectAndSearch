using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace SelectAndSearch.Benchmark {
    internal class Program {
        public static void BenchmarkTest() {
            BenchmarkRunner.Run<OCRHookBenchmark>();
        }

        static void Main(string[] args) {
            Console.WriteLine("Hello, World!");
            BenchmarkTest();
        }
    }
}