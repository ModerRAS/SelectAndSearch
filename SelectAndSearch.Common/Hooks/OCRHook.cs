using System.Runtime.InteropServices;
using Point = System.Drawing.Point;
using SelectAndSearch.Common.Interfaces;
using SelectAndSearch.Common.Services;
using SearchOption = SelectAndSearch.Common.Models.SearchOption;
using SelectAndSearch.Common.Enums;
using SelectAndSearch.Common.API;
using SelectAndSearch.Common.Models;
using static SelectAndSearch.Common.Interfaces.IHook;
using System.Diagnostics;

namespace SelectAndSearch.Common.Hooks {
    public class OCRHook : IHook {
        public SearchOption SearchOption { get; set; }
        public SearchService SearchService { get; set; }
        public IPopupForm PopupForm { get; set; }
        public GlobalConfig GlobalConfig { get; set; }
        public bool IsRunning { get; set; }
        public IOCR OCR { get; set; }
        public OCRHook(SearchService searchService, IPopupForm popupForm, GlobalConfig globalConfig, IOCR OCR) {
            this.OCR = OCR;
            if (searchService is null || popupForm is null || globalConfig is null) {
                return;
            }
            GlobalConfig = globalConfig;
            SearchService = searchService;
            PopupForm = popupForm;
            SearchOption = SearchService.option;
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

        public Point GetActualMousePostion(Point mousePosition) {
            var scale = GetDPIScaling();
            var actualMousePoint = new Point((int)(mousePosition.X * scale), (int)(mousePosition.Y * scale));
            return actualMousePoint;
        }
        public void Update() {
            var cap = GetScreenCapture();
            OCR.GetOcrResult(cap);
        }
        public void Execute() {
            var screenPoint = Control.MousePosition;//鼠标相对于屏幕左上角的坐标
            var actualMousePoint = GetActualMousePostion(screenPoint);
            var text = OCR.GetScreenText(actualMousePoint);
            SearchOption.SearchText = text;
            SearchOption.Skip = 0;
            SearchOption.Take = 20;
            PopupForm.ShowForm(screenPoint);
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
                        var UseTime = (StopTime - StartTime).Milliseconds;
                        var DelayTime = 2000 - UseTime;
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
