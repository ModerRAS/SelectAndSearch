using SelectAndSearch.Common.Managers;
using SearchOption = SelectAndSearch.Common.Models.SearchOption;

namespace SelectAndSearch.Common.Services {
    public class SearchService {
        public LuceneManager luceneManager { get; set; }
        public static SearchOption option { get; set; } = new SearchOption();
        public SearchService(LuceneManager luceneManager) {
            this.luceneManager = luceneManager;
        }
        public SearchOption Execute() {
            var (length, result) = luceneManager.Search(option.SearchText, option.Skip, option.Take);
            option.Skip += option.Take;
            option.Count = length;
            option.Questions = result;
            return new SearchOption() {
                SearchText = option.SearchText,
                Count = option.Count,
                Questions = option.Questions,
                Skip = option.Skip,
                Take = option.Take
            };
        }
    }
}
