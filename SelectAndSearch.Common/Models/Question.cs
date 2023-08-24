using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.Models {
    public class Question {
        public string Title { get; set; } = string.Empty;
        public QuestionType Type {
            get {
                if (Answers.Count <= 2) {
                    return QuestionType.Judgement;
                }
                if (CorrectAnswer.Length == 1) {
                    return QuestionType.Single;
                }
                if (Regex.IsMatch(CorrectAnswer.ToUpper(), "[A-Z]+") && CorrectAnswer.Length > 1) {
                    return QuestionType.Multiple;
                }
                return QuestionType.ShortAnswerQuestion;
            }
        }
        public List<string> Answers { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public List<int> CorrectAnswerNumber {
            get {
                var answerNumber = new List<int>();
                if (Regex.IsMatch(CorrectAnswer.ToUpper(), "[A-Z]+")) {
                    foreach (var item in CorrectAnswer.ToUpper()) {
                        answerNumber.Add(item - 'A');
                    }
                }
                return answerNumber;
            }
        }
    }
}
