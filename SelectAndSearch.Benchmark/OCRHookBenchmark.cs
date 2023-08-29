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
        public OCRHookBenchmark() {
            ocrMkl = new OCRHook(null, null, null);
            ocrMkl.InitOCR();
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
            ocrMkl.Update();
            //ocrMkl.GetScreenText();
        }
        [Benchmark]
        public void BenchmarkOCRWithInit() {
            var ocr2 = new OCRHook(null, null, null);
            ocr2.InitOCR();
            ocr2.Update();
            //ocr2.GetScreenText();
        }
    }
}
