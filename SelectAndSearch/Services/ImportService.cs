using SelectAndSearch.Managers;
using SelectAndSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Services {
    public class ImportService {
        public ExcelManager ExcelManager { get; set; }
        public LuceneManager LuceneManager { get; set; }
        public ImportService(ExcelManager excelManager, LuceneManager luceneManager) { 
            ExcelManager = excelManager;
            LuceneManager = luceneManager;
        }
        public async Task Execute(List<string> paths) {
            await Task.Run(() => {
                var questions = new List<Question>();
                foreach (var path in paths) {
                    questions.AddRange(ExcelManager.ParseDocument(path));
                }
                LuceneManager.WriteDocuments(questions);
            });
        }
    }
}
