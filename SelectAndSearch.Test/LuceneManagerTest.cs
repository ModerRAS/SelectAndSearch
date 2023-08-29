using SelectAndSearch.Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Test {
    [TestClass]
    public class LuceneManagerTest {
        [TestMethod]
        public void TestGetKeyWords_1() {
            var manager = new LuceneManager();
            var keywords = manager.GetKeyWords("题18/600、《网络安全紧急告警分析报告》内容应包含（）施等。");
            CollectionAssert.AreEqual(
                new List<string>() {"题","18","600","网络","安全","紧急","告警","分析","报告","内容","应","包含","施","等"},
                keywords);
            keywords.Select(keyword => {
                Console.WriteLine(keyword);
                return keyword;
            }).ToList();
            //"题18/600、\r\n《网络安全紧急告警分析报告》内容应包含（）施等。"
        }
        [TestMethod]
        public void TestGetKeyWords_2() {
            var manager = new LuceneManager();
            var keywords = manager.GetKeyWords("题18/600、\r\n《网络安全紧急告警分析报告》内容应包含（）施等。");
            CollectionAssert.AreEqual(
                new List<string>() { "题","18","600","网络","安全","紧急","告警","分析","报告","内容","应","包含","施","等" },
                keywords);
            keywords.Select(keyword => {
                Console.WriteLine(keyword);
                return keyword;
            }).ToList();
            //"1/400、安全扫描可以实现（）"
        }
        [TestMethod]
        public void TestGetKeyWords_3() {
            var manager = new LuceneManager();
            var keywords = manager.GetKeyWords("1/400、安全扫描可以实现（）");
            CollectionAssert.AreEqual(
                new List<string>() { "1", "400", "安全", "扫描", "可以", "实现" },
                keywords);
            keywords.Select(keyword => {
                Console.WriteLine(keyword);
                return keyword;
            }).ToList();
            //"1/400、安全扫描可以实现（）"
        }
    }
}
