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
        public OCRHook ocrMkl { get; set; }
        public OCRHook ocrOpenBlas { get; set; }
        public OCRHookBenchmark() {
            ocrMkl = new OCRHook();
            ocrOpenBlas = new OCRHook();
            ocrMkl.InitOCR();
            ocrOpenBlas.InitOCROpenBlas();
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
        public void BenchmarkOCRMkl() {
            ocrMkl.GetScreenText();
        }
        [Benchmark]
        public void BenchmarkOCRMklWithInit() {
            var ocr2 = new OCRHook();
            ocr2.InitOCR();
            ocr2.GetScreenText();
        }
        [Benchmark]
        public void BenchmarkOCROpenBlas() {
            ocrOpenBlas.GetScreenText();
        }
        [Benchmark]
        public void BenchmarkOCROpenBlasWithInit() {
            var ocr2 = new OCRHook();
            ocr2.InitOCROpenBlas();
            ocr2.GetScreenText();
        }
    }
}
