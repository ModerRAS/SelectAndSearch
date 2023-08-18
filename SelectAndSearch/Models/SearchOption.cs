using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Models {
    public class SearchOption {
        public string SearchText { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public int Count { get; set; }
        public List<Question> Questions { get; set; }
    }
}
