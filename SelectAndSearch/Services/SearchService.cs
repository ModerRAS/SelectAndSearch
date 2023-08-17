﻿using Ninject;
using SelectAndSearch.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Services {
    public class SearchService {
        [Inject]
        public LuceneManager luceneManager { get; set; }
        public SearchService() { }
        public string Execute(string SearchText) {
            var (length, result) = luceneManager.Search(SearchText, 0, 20);
            return "";
        }
    }
}
