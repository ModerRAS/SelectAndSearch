using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Sdcb.PaddleInference;

namespace SelectAndSearch.Benchmark {
    internal class Program {
        public static void BenchmarkTest() {
            BenchmarkRunner.Run<OCRHookBenchmark>();
        }

        static void Main(string[] args) {
            BenchmarkTest();
        }
    }
}