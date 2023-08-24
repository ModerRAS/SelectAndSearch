using SelectAndSearch.Common.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Test {
    [TestClass]
    public class OCRHookTest {
        [TestMethod]
        public void TestOCR() {
            var ocr = new OCRHook();
            Task.Delay(1000).Wait();
            ocr.Execute();
        }
        [TestMethod]
        public void BenchmarkOCR() {
            var ocr = new OCRHook();
            for (int i = 0; i < 60; i++) {
                ocr.Execute();
            }
        }
    }
}
