using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.OCR.RapidOCR {
    public class OcrData {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("data")]
        public List<Data> Datas { get; set; }
    }
}
