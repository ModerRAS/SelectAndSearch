using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleOCR.Models;
using SelectAndSearch.Common.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using Point = System.Drawing.Point;

namespace SelectAndSearch.Common.OCR.PaddleOCR {
    public class PaddleOCR : IOCR {
        public PaddleOcrAll all { get; set; }
        public PaddleOcrResult Result { get; set; }
        public PaddleOCR() {
            FullOcrModel model = LocalFullModels.ChineseV3;

            all = new PaddleOcrAll(model,
                PaddleDevice.Mkldnn()
                ) {
                AllowRotateDetection = true, /* 允许识别有角度的文字 */
                Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
            };
        }
        ~PaddleOCR() {
            all.Dispose();
        }
        public void GetOcrResult(Bitmap cap) {
            var stream = new MemoryStream();
            cap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            using (Mat src = Cv2.ImDecode(stream.ToArray(), ImreadModes.Color)) {
                Result = all.Run(src);
            }
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
        public string GetScreenText(Point point) {
            if (Result is null) {
                return string.Empty;
            }
            foreach (PaddleOcrResultRegion region in Result.Regions) {
                var points = region.Rect.Points().Select(e => {
                    return new Point((int)e.X, (int)e.Y);
                }).ToArray();

                if (IsPointInConvexPolygon(points, point)) {
                    Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                    return region.Text;
                }

            }
            return string.Empty;
        }

    }
}