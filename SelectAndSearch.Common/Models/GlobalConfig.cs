using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SearchOption = SelectAndSearch.Common.Models.SearchOption;

namespace SelectAndSearch.Common.Models {
    public class GlobalConfig {
        public Keys OCRBindingKey { get; set; }
        public SearchOption SearchOption { get; set; }
    }
}
