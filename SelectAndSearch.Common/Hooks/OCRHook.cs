using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleOCR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using OpenCvSharp;
using Point = System.Drawing.Point;
using SkiaSharp;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using SelectAndSearch.Common.Interfaces;
using SelectAndSearch.Common.Services;
using SearchOption = SelectAndSearch.Common.Models.SearchOption;

namespace SelectAndSearch.Common.Hooks {
    public class OCRHook {
        public PaddleOcrAll all { get; set; }
        public SearchOption SearchOption { get; set; }
        public SearchService SearchService { get; set; }
        public IPopupForm PopupForm { get; set; }
        public OCRHook() {
        }
        public OCRHook(SearchService searchService, IPopupForm popupForm) {
            SearchService = searchService;
            PopupForm = popupForm;
            SearchOption = SearchService.option;
            InitOCR();
        }
        public void InitOCR() {
            FullOcrModel model = LocalFullModels.ChineseV3;
            all = new PaddleOcrAll(model, PaddleDevice.Mkldnn()) {
                AllowRotateDetection = true, /* 允许识别有角度的文字 */
                Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
            };
        }
        public void InitOCROpenBlas() {
            FullOcrModel model = LocalFullModels.ChineseV3;
            all = new PaddleOcrAll(model, PaddleDevice.Openblas()) {
                AllowRotateDetection = true, /* 允许识别有角度的文字 */
                Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
            };
        }
        ~OCRHook() {
            all.Dispose();
        }
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap {
            /// <summary>
            /// Device driver version
            /// </summary>
            DRIVERVERSION = 0,
            /// <summary>
            /// Device classification
            /// </summary>
            TECHNOLOGY = 2,
            /// <summary>
            /// Horizontal size in millimeters
            /// </summary>
            HORZSIZE = 4,
            /// <summary>
            /// Vertical size in millimeters
            /// </summary>
            VERTSIZE = 6,
            /// <summary>
            /// Horizontal width in pixels
            /// </summary>
            HORZRES = 8,
            /// <summary>
            /// Vertical height in pixels
            /// </summary>
            VERTRES = 10,
            /// <summary>
            /// Number of bits per pixel
            /// </summary>
            BITSPIXEL = 12,
            /// <summary>
            /// Number of planes
            /// </summary>
            PLANES = 14,
            /// <summary>
            /// Number of brushes the device has
            /// </summary>
            NUMBRUSHES = 16,
            /// <summary>
            /// Number of pens the device has
            /// </summary>
            NUMPENS = 18,
            /// <summary>
            /// Number of markers the device has
            /// </summary>
            NUMMARKERS = 20,
            /// <summary>
            /// Number of fonts the device has
            /// </summary>
            NUMFONTS = 22,
            /// <summary>
            /// Number of colors the device supports
            /// </summary>
            NUMCOLORS = 24,
            /// <summary>
            /// Size required for device descriptor
            /// </summary>
            PDEVICESIZE = 26,
            /// <summary>
            /// Curve capabilities
            /// </summary>
            CURVECAPS = 28,
            /// <summary>
            /// Line capabilities
            /// </summary>
            LINECAPS = 30,
            /// <summary>
            /// Polygonal capabilities
            /// </summary>
            POLYGONALCAPS = 32,
            /// <summary>
            /// Text capabilities
            /// </summary>
            TEXTCAPS = 34,
            /// <summary>
            /// Clipping capabilities
            /// </summary>
            CLIPCAPS = 36,
            /// <summary>
            /// Bitblt capabilities
            /// </summary>
            RASTERCAPS = 38,
            /// <summary>
            /// Length of the X leg
            /// </summary>
            ASPECTX = 40,
            /// <summary>
            /// Length of the Y leg
            /// </summary>
            ASPECTY = 42,
            /// <summary>
            /// Length of the hypotenuse
            /// </summary>
            ASPECTXY = 44,
            /// <summary>
            /// Shading and Blending caps
            /// </summary>
            SHADEBLENDCAPS = 45,

            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,
            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90,

            /// <summary>
            /// Number of entries in physical palette
            /// </summary>
            SIZEPALETTE = 104,
            /// <summary>
            /// Number of reserved entries in palette
            /// </summary>
            NUMRESERVED = 106,
            /// <summary>
            /// Actual color resolution
            /// </summary>
            COLORRES = 108,

            // Printing related DeviceCaps. These replace the appropriate Escapes
            /// <summary>
            /// Physical Width in device units
            /// </summary>
            PHYSICALWIDTH = 110,
            /// <summary>
            /// Physical Height in device units
            /// </summary>
            PHYSICALHEIGHT = 111,
            /// <summary>
            /// Physical Printable Area x margin
            /// </summary>
            PHYSICALOFFSETX = 112,
            /// <summary>
            /// Physical Printable Area y margin
            /// </summary>
            PHYSICALOFFSETY = 113,
            /// <summary>
            /// Scaling factor x
            /// </summary>
            SCALINGFACTORX = 114,
            /// <summary>
            /// Scaling factor y
            /// </summary>
            SCALINGFACTORY = 115,

            /// <summary>
            /// Current vertical refresh rate of the display device (for displays only) in Hz
            /// </summary>
            VREFRESH = 116,
            /// <summary>
            /// Vertical height of entire desktop in pixels
            /// </summary>
            DESKTOPVERTRES = 117,
            /// <summary>
            /// Horizontal width of entire desktop in pixels
            /// </summary>
            DESKTOPHORZRES = 118,
            /// <summary>
            /// Preferred blt alignment
            /// </summary>
            BLTALIGNMENT = 119
            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }


        public static (int, int) GetScreenSize() {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            int PhysicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPHORZRES);
            g.ReleaseHdc();
            return (PhysicalScreenWidth, PhysicalScreenHeight); // 1.25 = 125%
        }

        public static float GetDPIScaling() {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }

        public static Bitmap GetScreenCapture() {
            var (width, height) = GetScreenSize();
            Rectangle tScreenRect = new Rectangle(0, 0, width, height);
            Bitmap tSrcBmp = new Bitmap(width, height); // 用于屏幕原始图片保存
            Graphics gp = Graphics.FromImage(tSrcBmp);
            gp.CopyFromScreen(0, 0, 0, 0, tScreenRect.Size);
            gp.DrawImage(tSrcBmp, 0, 0, tScreenRect, GraphicsUnit.Pixel);
            return tSrcBmp;
        }

        static bool IsPointInConvexPolygon(Point[] polygon, Point point) {
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
        public (Point, string) GetScreenText() {
            var screenPoint = Control.MousePosition;//鼠标相对于屏幕左上角的坐标
            var scale = GetDPIScaling();
            var actualMousePoint = new Point((int)(screenPoint.X * scale), (int)(screenPoint.Y * scale));
            Console.WriteLine($"{screenPoint.X}, {screenPoint.Y}, {scale}");
            var cap = GetScreenCapture();
            var stream = new MemoryStream();
            cap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            using (Mat src = Cv2.ImDecode(stream.ToArray(), ImreadModes.Color)) {
                PaddleOcrResult result = all.Run(src);
                //Console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions) {
                    var points = region.Rect.Points().Select(e => {
                        return new Point((int)e.X, (int)e.Y);
                    }).ToArray();

                    if (IsPointInConvexPolygon(points, actualMousePoint)) {
                        Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                        return (screenPoint, region.Text);
                    }

                }
            }
            return (screenPoint, string.Empty);
        }
        public void Execute() {
            var text = GetScreenText();
            SearchOption.SearchText = text.Item2;
            SearchOption.Skip = 0;
            SearchOption.Take = 20;
            PopupForm.ShowForm(text.Item1);
        }
    }
}
