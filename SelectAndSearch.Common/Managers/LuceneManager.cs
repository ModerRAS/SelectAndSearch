using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Cn.Smart;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using SelectAndSearch.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Reflection.Expressions;

namespace SelectAndSearch.Common.Managers {
    public class LuceneManager {
        public string WorkDir { get; set; }
        public string IndexDir { get; set; }
        public LuceneManager() {
            WorkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SelectAndSearch");
            IndexDir = Path.Combine(WorkDir, "Index_Data");
            if (!System.IO.Directory.Exists(WorkDir)) {
                System.IO.Directory.CreateDirectory(WorkDir);
            }
        }
        public bool GenerateDocument(Question question, out Document doc) {
            doc = new Document();
            if (string.IsNullOrWhiteSpace(question.Title)) {
                return false;
            }
            var title = new TextField("Title", question.Title, Field.Store.YES);
            title.Boost = 1F;
            doc.Add(title);

            var questionTypeField = new Int32Field("QuestionType", (int)question.Type, Field.Store.YES);
            doc.Add(questionTypeField);

            TextField AnswersField = new TextField("Answers", JsonConvert.SerializeObject(question.Answers), Field.Store.YES);
            AnswersField.Boost = 1F;
            doc.Add(AnswersField);


            var correctAnswer = new TextField("CorrectAnswer", question.CorrectAnswer, Field.Store.YES);
            correctAnswer.Boost = 1F;
            doc.Add(correctAnswer);

            var remark = new TextField("Remark", question.Remark, Field.Store.YES);
            remark.Boost = 1F;
            doc.Add(remark);

            var indexedText = new TextField("IndexedText", JsonConvert.SerializeObject(question), Field.Store.YES);
            indexedText.Boost = 1F;
            doc.Add(indexedText);

            return true;
        }
        public async Task WriteDocumentAsync(Question question) {
            using (var writer = GetIndexWriter()) {

                try {
                    if (GenerateDocument(question, out var doc)) {
                        writer.AddDocument(doc);
                    }
                    writer.Flush(triggerMerge: true, applyAllDeletes: true);
                    writer.Commit();
                } catch (ArgumentNullException ex) {
                }
            }
        }
        public void WriteDocuments(IEnumerable<Question> questions) {
            using (var writer = GetIndexWriter()) {
                foreach (Question question in questions) {
                    try {
                        if (GenerateDocument(question, out var doc)) {
                            writer.AddDocument(doc);
                        }
                    } catch (ArgumentNullException ex) {
                        Console.WriteLine(ex);
                    }

                }
                writer.Flush(triggerMerge: true, applyAllDeletes: true);
                writer.Commit();
            }

        }
        private IndexWriter GetIndexWriter() {
            var dir = FSDirectory.Open(IndexDir);
            Analyzer analyzer = new SmartChineseAnalyzer(LuceneVersion.LUCENE_48);
            var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            IndexWriter writer = new IndexWriter(dir, indexConfig);
            return writer;
        }

        public List<string> GetKeyWords(string q) {
            List<string> keyworkds = new List<string>();
            Analyzer analyzer = new SmartChineseAnalyzer(LuceneVersion.LUCENE_48);
            using (var ts = analyzer.GetTokenStream(null, q)) {
                ts.Reset();
                var ct = ts.GetAttribute<Lucene.Net.Analysis.TokenAttributes.ICharTermAttribute>();

                while (ts.IncrementToken()) {
                    StringBuilder keyword = new StringBuilder();
                    for (int i = 0; i < ct.Length; i++) {
                        keyword.Append(ct.Buffer[i]);
                    }
                    string item = keyword.ToString();
                    if (!keyworkds.Contains(item)) {
                        keyworkds.Add(item);
                    }
                }
            }
            return keyworkds;
        }
        public (int, ScoreDoc[]) RecursiveSearch(IEnumerable<string> q, int Skip, int Take, IndexSearcher searcher) {
            var items = q.SkipWhile(x => float.TryParse(x, out var num));
            var keyWordQuery = new BooleanQuery();
            foreach (var item in items) {
                keyWordQuery.Add(new TermQuery(new Term("IndexedText", item)), Occur.SHOULD);
            }
            var top = searcher.Search(keyWordQuery, Skip + Take);
            var total = top.TotalHits;
            var hits = top.ScoreDocs;
            if (total > 0) {
                return (total, hits);
            } else {
                return RecursiveSearch(items.Skip(1).Take(items.ToList().Count - 2), Skip, Take, searcher);
            }
        }
        public (int, List<Question>) Search(string q, int Skip, int Take) {
            IndexReader reader = DirectoryReader.Open(FSDirectory.Open(IndexDir));

            var searcher = new IndexSearcher(reader);

            var items = GetKeyWords(q).SkipWhile(x => float.TryParse(x, out var num));
            var keyWordQuery = new BooleanQuery();
            foreach (var item in items) {
                keyWordQuery.Add(new TermQuery(new Term("IndexedText", item)), Occur.SHOULD);
            }
            var top = searcher.Search(keyWordQuery, Skip + Take);
            var total = top.TotalHits;
            var hits = top.ScoreDocs;

            var question = new List<Question>();
            var id = 0;
            foreach (var hit in hits) {
                if (id++ < Skip) continue;
                var document = searcher.Doc(hit.Doc);
                var per = JsonConvert.DeserializeObject<Question>(document.Get("IndexedText"));
                if (per.Contain(question)) { continue; }
                else {
                    question.Add(per);
                }
            }
            return (total, question.ToList());
        }
    }
}
