using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SelectAndSearch.Models {
    public class Question {
        public string Title { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public List<string> Answers { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public List<int> CorrectAnswerNumber { get { 
                var answerNumber = new List<int>();
                if (Regex.IsMatch(CorrectAnswer.ToUpper(), "[A-Z]+")) {
                    foreach (var item in CorrectAnswer.ToUpper()) {
                        answerNumber.Add(item - 'A');
                    }
                }
                return answerNumber;
            } }
    }
}
