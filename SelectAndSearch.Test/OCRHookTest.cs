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
            var ocr = new OCRHook(null, null, null);
            ocr.InitOCR();
            Task.Delay(1000).Wait();
            ocr.Update();
            ocr.GetScreenText();
        }
    }
}
