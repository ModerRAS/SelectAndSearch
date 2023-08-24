using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelectAndSearch.Common.Hooks;
using BenchmarkDotNet.Jobs;

namespace SelectAndSearch.Benchmark {
    [RPlotExporter]
    public class OCRHookBenchmark {
        public OCRHook ocr { get; set; }
        public OCRHookBenchmark() {
            ocr = new OCRHook();
        }
        [Benchmark]
        public void BenchmarkGetScreenSize() {
            var (x, y) = OCRHook.GetScreenSize();
        }
        [Benchmark]
        public void BenchmarkGetScreenCapture() {
            var bitmap = OCRHook.GetScreenCapture();
        }
        [Benchmark]
        public void BenchmarkGetDPIScaling() {
            var tmp = OCRHook.GetDPIScaling();
        }
        [Benchmark]
        public void BenchmarkOCR() {
            ocr.GetScreenText();
        }
        [Benchmark]
        public void BenchmarkOCRWithInit() {
            var ocr2 = new OCRHook();
            ocr2.GetScreenText();
        }
    }
}
