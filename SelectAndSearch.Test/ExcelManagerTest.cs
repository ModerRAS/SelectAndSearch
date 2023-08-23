using SelectAndSearch.Managers;

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
            Assert.AreEqual("�������", questions[0].Title);
            Assert.AreEqual("A", questions[0].CorrectAnswer);
            Assert.AreEqual("ѡ��A", questions[0].Answers[0]);
            Assert.AreEqual("ѡ��B", questions[0].Answers[1]);
            Assert.AreEqual("ѡ��C", questions[0].Answers[2]);
            Assert.AreEqual(3, questions[0].Answers.Count);
        }
    }
}