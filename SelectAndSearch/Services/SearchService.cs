using SelectAndSearch.Managers;
using SelectAndSearch.Models;
using SearchOption = SelectAndSearch.Models.SearchOption;

namespace SelectAndSearch.Services {
    public class SearchService {
        public LuceneManager luceneManager { get; set; }
        public SearchService(LuceneManager luceneManager) { 
            this.luceneManager = luceneManager;
        }
        public SearchOption Execute(SearchOption option) {
            var (length, result) = luceneManager.Search(option.SearchText, option.Skip, option.Take);
            option.Count = length;
            option.Questions = result;
            return option;
        }
    }
}
