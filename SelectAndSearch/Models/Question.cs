using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Models {
    public class Question {
        public string Title { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public List<string> Answers { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
    }
}
