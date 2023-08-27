using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Sdcb.PaddleInference;

namespace SelectAndSearch.Benchmark {
    internal class Program {
        public static void BenchmarkTest() {
            BenchmarkRunner.Run<OCRHookBenchmark>(DefaultConfig.Instance.With(
                    Job.Default.WithCustomBuildConfiguration(
#if DEBUG || RELEASE_MKL || RELEASE
                "Release_mkl"
#elif RELEASE_OPENBLAS
                "Release_openblas"
#elif RELEASE_OPENBLAS_NOAVX
                "Release_openblas_noavx"
#endif
                        )
                    ));
        }

        static void Main(string[] args) {
            BenchmarkTest();
        }
    }
}