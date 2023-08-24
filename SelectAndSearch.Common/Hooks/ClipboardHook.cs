using Newtonsoft.Json;
using SelectAndSearch.Common.Interfaces;
using SelectAndSearch.Common.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SearchOption = SelectAndSearch.Common.Models.SearchOption;

namespace SelectAndSearch.Common.Hooks {
    public class ClipboardHook {
        public SearchOption SearchOption { get; set; }
        public SearchService SearchService { get; set; }
        public IPopupForm PopupForm { get; set; }
        public ClipboardHook(SearchService searchService, IPopupForm popupForm) {
            SearchService = searchService;
            PopupForm = popupForm;
            SearchOption = SearchService.option;
        }
        #region 变量

        /// <summary>
        /// 声明委托
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        public delegate int GlobalHookProc(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// 声明键盘钩子事件类型
        /// </summary>
        HookProc KeyboardHookProcedure;

        /// <summary>
        /// 键盘钩子句柄
        /// </summary>
        int hKeyboardHook = 0;

        /// <summary>
        /// 定义鼠标钩子句柄.
        /// </summary>
        int hMouseHook = 0;


        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "FindWindowA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //装置钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //卸下钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //获取某个进程的句柄函数
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// 使用这个函数钩信息传递给链中的下一个钩子过程。
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);


        #region 引用win32api方法

        /// <summary>
        /// 导入模拟键盘的方法
        /// </summary>
        /// <param name="bVk" >按键的虚拟键值</param>
        /// <param name= "bScan" >扫描码，一般不用设置，用0代替就行</param>
        /// <param name= "dwFlags" >选项标志：0：表示按下，2：表示松开</param>
        /// <param name= "dwExtraInfo">一般设置为0</param>
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        #endregion

        /// <summary>
        /// 普通按键消息
        /// </summary>
        private const int WM_KEYDOWN = 0x100;
        /// <summary>
        /// 系统按键消息
        /// </summary>
        private const int WM_SYSKEYDOWN = 0x104;

        //鼠标常量 
        public const int WH_MOUSE_LL = 14;//low level mouse event
        public const int WH_KEYBOARD_LL = 13;

        //声明键盘钩子的封送结构类型 
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct {
            public int vkCode; //表示一个在1到254间的虚似键盘码 
            public int scanCode; //表示硬件扫描码 
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        //Declare wrapper managed POINT class.
        [StructLayout(LayoutKind.Sequential)]
        public class POINT {
            public int x;
            public int y;
        }

        //Declare wrapper managed MouseHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }


        #endregion




        /// <summary>
        /// 启动键盘钩子
        /// </summary>
        public void Start_Hook() {
            CopyTimes = 0;
            //启动键盘钩子 
            if (hKeyboardHook == 0) {
                //实例化委托
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                Process curProcess = Process.GetCurrentProcess();
                ProcessModule curModule = curProcess.MainModule;
                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(curModule.ModuleName), 0);
            }


        }

        /// <summary>
        /// 停止键盘钩子
        /// </summary>
        public void Stop_Hook() {
            bool retKeyboard = true;

            if (hKeyboardHook != 0) {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            if (hMouseHook != 0) {
                retKeyboard = UnhookWindowsHookEx(hMouseHook);
                hMouseHook = 0;
            }
            //如果卸下钩子失败 
            if (!retKeyboard) throw new Exception("卸下钩子失败！");
        }

        public static byte vbKeyControl = 0x11;  // CTRL 键
        public static byte vbKeyC = 67;
        public static byte vbKeyZ = 90;
        public static int CopyTimes = 0;
        /// <summary>
        /// 截取全局按键，发送新按键，返回
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;

                //截获CONTROL+C 
                if (keyData.ToString() == "C" && Control.ModifierKeys == Keys.Control) {
                    CopyTimes++;
                    // 防止第二次按下时获取剪切板文本失败导致后续不可用
                    if (CopyTimes > 2) {
                        CopyTimes = 0;
                    }
                    if (Clipboard.ContainsText(TextDataFormat.Text) && CopyTimes == 2) {
                        string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                        SearchOption.Take = 20;
                        SearchOption.Skip = 0;
                        SearchOption.SearchText = clipboardText;

                        Point screenPoint = Control.MousePosition;//鼠标相对于屏幕左上角的坐标

                        Point point = new Point(screenPoint.X, screenPoint.Y);//窗体位置
                        PopupForm.ShowForm(point);

                        CopyTimes = 0;
                    }

                    //return为了屏蔽原来的按键，如果去掉，则原来的按键和新的按键都会模拟按。
                    //return 1;
                } else if (keyData.ToString() == "Z") {


                }




            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            //return 0;
        }
    }
}
