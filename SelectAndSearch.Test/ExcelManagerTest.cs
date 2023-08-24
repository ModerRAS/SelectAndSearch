using SelectAndSearch.Common.Managers;

namespace SelectAndSearch.Test {
    [TestClass]
    public class ExcelManagerTest {
        public ExcelManagerTest() {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        [TestMethod]
        public void TestParseDocument() {
            var manager = new ExcelManager();
            var questions = manager.ParseDocument("TestData/ExcelManager/test1.xlsx");
            Assert.AreEqual(1, questions.Count);
            Assert.AreEqual("这是题干", questions[0].Title);
            Assert.AreEqual("A", questions[0].CorrectAnswer);
            Assert.AreEqual("选项A", questions[0].Answers[0]);
            Assert.AreEqual("选项B", questions[0].Answers[1]);
            Assert.AreEqual("选项C", questions[0].Answers[2]);
            Assert.AreEqual(3, questions[0].Answers.Count);
        }
    }
}