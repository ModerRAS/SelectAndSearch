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
using SelectAndSearch.Common.Enums;
using SelectAndSearch.Common.API;
using SelectAndSearch.Common.Models;
using static SelectAndSearch.Common.Hooks.ClipboardHook;
using static SelectAndSearch.Common.Interfaces.IHook;
using System.Diagnostics;

namespace SelectAndSearch.Common.Hooks {
    public class OCRHook : IHook {
        public PaddleOcrAll all { get; set; }
        public SearchOption SearchOption { get; set; }
        public SearchService SearchService { get; set; }
        public IPopupForm PopupForm { get; set; }
        public GlobalConfig GlobalConfig { get; set; }
        public PaddleOcrResult OcrResult { get; set; }
        public bool IsRunning { get; set; }
        public OCRHook(SearchService searchService, IPopupForm popupForm, GlobalConfig globalConfig) {
            if (searchService is null || popupForm is null || globalConfig is null) {
                return;
            }
            GlobalConfig = globalConfig;
            SearchService = searchService;
            PopupForm = popupForm;
            SearchOption = SearchService.option;
            InitOCR();
        }
        public void InitOCR() {
            FullOcrModel model = LocalFullModels.ChineseV3;

            all = new PaddleOcrAll(model,
#if DEBUG || RELEASE_MKL || RELEASE
                PaddleDevice.Mkldnn()
#elif RELEASE_CUDA118_CUDNN86_TR85_SM86_89 || RELEASE_CUDA102_CUDNN76_TR72_SM61_75
                PaddleDevice.Gpu().And(PaddleDevice.TensorRt("det.txt")),
                PaddleDevice.Gpu().And(PaddleDevice.TensorRt("cls.txt")),
                PaddleDevice.Gpu().And(PaddleDevice.TensorRt("rec.txt"))
#elif RELEASE_OPENBLAS || RELEASE_OPENBLAS_NOAVX
                PaddleDevice.Openblas()
#endif
                ) {
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


        public static (int, int) GetScreenSize() {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int PhysicalScreenHeight = Win32API.GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            int PhysicalScreenWidth = Win32API.GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPHORZRES);
            g.ReleaseHdc();
            return (PhysicalScreenWidth, PhysicalScreenHeight); // 1.25 = 125%
        }

        public static float GetDPIScaling() {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = Win32API.GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = Win32API.GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

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
        public PaddleOcrResult GetOcrResult() {
            var cap = GetScreenCapture();
            var stream = new MemoryStream();
            cap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            using (Mat src = Cv2.ImDecode(stream.ToArray(), ImreadModes.Color)) {
                PaddleOcrResult result = all.Run(src);
                return result;
            }
        }
        public Point GetActualMousePostion(Point mousePosition) {
            var scale = GetDPIScaling();
            var actualMousePoint = new Point((int)(mousePosition.X * scale), (int)(mousePosition.Y * scale));
            return actualMousePoint;
        }
        public void Update() {
            OcrResult = GetOcrResult();
        }
        public (Point, string) GetScreenText() {
            var screenPoint = Control.MousePosition;//鼠标相对于屏幕左上角的坐标
            if (OcrResult is null) {
                return (screenPoint, string.Empty);
            }
            var actualMousePoint = GetActualMousePostion(screenPoint);
            var result = OcrResult;
            foreach (PaddleOcrResultRegion region in result.Regions) {
                var points = region.Rect.Points().Select(e => {
                    return new Point((int)e.X, (int)e.Y);
                }).ToArray();

                if (IsPointInConvexPolygon(points, actualMousePoint)) {
                    Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                    return (screenPoint, region.Text);
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

        private bool MatchKeys(Keys expectedKeys, Keys actualKeys) {
            return (expectedKeys & Keys.Modifiers) == (actualKeys & Keys.Modifiers) &&
                   (expectedKeys & Keys.KeyCode) == (actualKeys & Keys.KeyCode);
        }
        /// <summary>
        /// 键盘钩子句柄
        /// </summary>
        int hKeyboardHook = 0;
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == (int)KeyboardKeyEvent.WM_KEYDOWN || wParam == (int)KeyboardKeyEvent.WM_SYSKEYDOWN) {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                if (MatchKeys(GlobalConfig.OCRBindingKey, keyData | Control.ModifierKeys)) {
                    Execute();
                }
                //截获CONTROL+C 
                //if (keyData.ToString() == "C" && Control.ModifierKeys == Keys.Control) {
                    
                //} 




            }
            return Win32API.CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            //return 0;
        }
        /// <summary>
        /// 声明键盘钩子事件类型
        /// </summary>
        HookProc KeyboardHookProcedure;
        public void StartHook() {
            if (hKeyboardHook == 0) {
                IsRunning = true;
                Task.Run(() => {
                    while (IsRunning) {
                        var StartTime = DateTime.Now;
                        Update();
                        var StopTime = DateTime.Now;
                        var DelayTime = 5000 - (StopTime - StartTime).Milliseconds;
                        Task.Delay(DelayTime > 0 ? DelayTime : 0).Wait();
                    }
                });
                //实例化委托
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                Process curProcess = Process.GetCurrentProcess();
                ProcessModule curModule = curProcess.MainModule;
                hKeyboardHook = Win32API.SetWindowsHookEx((int)KeyboardKeyEvent.WH_KEYBOARD_LL, KeyboardHookProcedure, Win32API.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void StopHook() {
            bool retKeyboard = true;
            IsRunning = false;

            if (hKeyboardHook != 0) {
                retKeyboard = Win32API.UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            //如果卸下钩子失败 
            if (!retKeyboard) throw new Exception("卸下钩子失败！");
        }
    }
}
