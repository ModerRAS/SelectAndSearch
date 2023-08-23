﻿using Newtonsoft.Json;
using SelectAndSearch.Models;
using SelectAndSearch.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SearchOption = SelectAndSearch.Models.SearchOption;

namespace SelectAndSearch.Hooks
{
    public class ClipboardHook
    {
        public SearchOption SearchOption { get; set; }
        public SearchService SearchService { get; set; }
        public PopupForm PopupForm { get; set; }
        public ClipboardHook(SearchService searchService, PopupForm popupForm) {
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
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        public delegate int GlobalHookProc(int nCode, Int32 wParam, IntPtr lParam);

        /// <summary>
        /// 声明键盘钩子事件类型
        /// </summary>
        HookProc KeyboardHookProcedure;
        HookProc MouseHookProcedure;

        /// <summary>
        /// 键盘钩子句柄
        /// </summary>
        int hKeyboardHook = 0;

        /// <summary>
        /// 定义鼠标钩子句柄.
        /// </summary>
        int hMouseHook = 0;


        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SetForegroundWindow(System.IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindowA", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

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
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);


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

            // install Mouse hook 
            if (hMouseHook == 0) {
                //实例化委托
                MouseHookProcedure = new HookProc(MouseHookProc);
                Process curProcess = Process.GetCurrentProcess();
                ProcessModule curModule = curProcess.MainModule;
                hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, GetModuleHandle(curModule.ModuleName), 0);


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
            if (!(retKeyboard)) throw new Exception("卸下钩子失败！");
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
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam) {
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
                        PopupForm.StartPosition = FormStartPosition.Manual;//窗体其实位置类型，manual由location指定
                        PopupForm.Location = point;
                        PopupForm.TopMost = true;
                        PopupForm.Visible = true;

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



        #region Win32变量

        private const int WM_MOUSEMOVE = 0x200;

        private const int WM_LBUTTONDOWN = 0x201;

        private const int WM_RBUTTONDOWN = 0x204;

        private const int WM_MBUTTONDOWN = 0x207;

        private const int WM_LBUTTONUP = 0x202;

        private const int WM_RBUTTONUP = 0x205;

        private const int WM_MBUTTONUP = 0x208;

        private const int WM_LBUTTONDBLCLK = 0x203;

        private const int WM_RBUTTONDBLCLK = 0x206;

        private const int WM_MBUTTONDBLCLK = 0x209;
        #endregion

        /// <summary>
        /// 截取鼠标按键
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam) {
            if (nCode >= 0) {
                //MouseButtons button = MouseButtons.None;
                switch (wParam) {
                    case WM_LBUTTONDOWN:    //左键按下
                                            //case WM_LBUTTONUP:        //右键按下
                                            //case WM_LBUTTONDBLCLK:    //同时按下
                        Hide_Form();
                        break;
                    case WM_LBUTTONUP:      // 左键松开
                        //模拟按下ctrl键
                        //keybd_event(vbKeyControl, 0, 0, 0);
                        ////模拟按下C键
                        //keybd_event(vbKeyC, 0, 0, 0);

                        ////模拟松开ctrl键
                        //keybd_event(vbKeyControl, 0, 2, 0);
                        ////模拟松开C键
                        //keybd_event(vbKeyC, 0, 2, 0);

                        break;
                }

            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }

        public void Hide_Form() {

            if (PopupForm != null && PopupForm.Visible) {
                Rectangle rect = PopupForm.Bounds;
                Rectangle mouseRect = new Rectangle(new Point(Control.MousePosition.X, Control.MousePosition.Y), new Size(5, 5));
                if (!rect.Contains(mouseRect)) {
                    PopupForm.Visible = false;
                }

            }

        }

        public void Print_MessageBox(string data) {
            Console.WriteLine(data);
        }
    }
}
