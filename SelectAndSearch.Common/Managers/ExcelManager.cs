﻿using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Microsoft.AspNetCore.Components.Forms;
using SelectAndSearch.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.Managers {
    public class ExcelManager {
        public ExcelManager() { }
        public Question ParsePerLine(IExcelDataReader reader) {
            var question = new Question() {
                Title = reader.GetString(0),
                CorrectAnswer = reader.GetString(1),
            };
            try {
                for (int i = 2; !string.IsNullOrWhiteSpace(reader.GetString(i)); i++) {
                    var answer = reader.GetString(i);
                    question.Answers.Add(answer);
                }
            } catch (IndexOutOfRangeException e) {
                Console.WriteLine(e);
            }

            return question;
        }
        public List<Question> ParseDocument(string path) {
            var questions = new List<Question>();
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read)) {
                using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                    do {
                        while (reader.Read()) {
                            try {
                                questions.Add(ParsePerLine(reader));
                            } catch (IndexOutOfRangeException e) {
                                Console.WriteLine(e);
                            }
                        }
                    } while (reader.NextResult());
                }
            }
            return questions;
        }
    }
}