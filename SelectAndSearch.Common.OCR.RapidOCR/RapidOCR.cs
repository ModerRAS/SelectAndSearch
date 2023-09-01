using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelectAndSearch.Common.Interfaces;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace SelectAndSearch.Common.OCR.RapidOCR {
    public class RapidOCR : IOCR {
        public OcrAPI Ocr { get; set; }
        public OcrData OcrData { get; set; }
        ~RapidOCR() {
            Ocr.Stop();
        }
        public RapidOCR() {
            string ocrPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RapidOCR-json", "RapidOCR_json.exe");
            if (!File.Exists(ocrPath)) {
                Console.WriteLine($"Engine not found at the following path:\n{ocrPath}");
                Console.ReadLine();
                Environment.Exit(1);
            }

            Ocr = new OcrAPI(ocrPath);
        }
        public bool IsPointInConvexPolygon(Point[] polygon, Point point) {
            int n = polygon.Length;
            if (n < 3)
                throw new ArgumentException("A convex polygon should have at least 3 vertices.");

            // Check if the point is on the same side of all edges.
            for (int i = 0; i < n; i++) {
                int next = (i + 1) % n;
                double crossProduct = (polygon[next].X - polygon[i].X) * (point.Y - polygon[i].Y) -
                                      (polygon[next].Y - polygon[i].Y) * (point.X - polygon[i].X);

                if (crossProduct < 0) // Point is on the wrong side of an edge.
                    return false;
            }

            return true; // Point is on the same side of all edges.
        }
        public void GetOcrResult(Bitmap cap) {
            var filename = $"{Path.GetTempFileName()}.png";
            cap.Save(filename);
            JObject res = Ocr.Run(filename);
            if (res.GetValue("code").Value<int>() == 100) {
                OcrData = res.ToObject<OcrData>();
            }
            
        }
        

        public string GetScreenText(Point point) {
            if (OcrData is null) {
                return string.Empty;
            }
            foreach (var region in OcrData.Datas) {
                var points = region.Points();

                if (IsPointInConvexPolygon(points, point)) {
                    Console.WriteLine($"Text: {region.Text}, Score: {region.Score}");
                    return region.Text;
                }

            }
            return string.Empty;
        }
    }
}
