using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.OCR.RapidOCR {
    public class Data {
        [JsonProperty("box")]
        public List<List<int>> Box { get; set; }
        [JsonProperty("score")]
        public float Score { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        public Point[] Points() {
            var points = new List<Point>();
            foreach (var e in Box) {
                if (e.Count == 2) {
                    points.Add(new Point(e[0], e[1]));
                }
            }
            return points.ToArray();
        }
    }
}
