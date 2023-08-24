using Newtonsoft.Json;
using SelectAndSearch.Common.API;
using SelectAndSearch.Common.Enums;
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
using static SelectAndSearch.Common.Interfaces.IHook;
using SearchOption = SelectAndSearch.Common.Models.SearchOption;

namespace SelectAndSearch.Common.Hooks {
    public class ClipboardHook : IHook {
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
        /// 声明键盘钩子事件类型
        /// </summary>
        HookProc KeyboardHookProcedure;

        /// <summary>
        /// 键盘钩子句柄
        /// </summary>
        int hKeyboardHook = 0;


        #endregion


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







        /// <summary>
        /// 启动键盘钩子
        /// </summary>
        public void StartHook() {
            CopyTimes = 0;
            //启动键盘钩子 
            if (hKeyboardHook == 0) {
                //实例化委托
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                Process curProcess = Process.GetCurrentProcess();
                ProcessModule curModule = curProcess.MainModule;
                hKeyboardHook = Win32API.SetWindowsHookEx((int)KeyboardKeyEvent.WH_KEYBOARD_LL, KeyboardHookProcedure, Win32API.GetModuleHandle(curModule.ModuleName), 0);
            }


        }

        /// <summary>
        /// 停止键盘钩子
        /// </summary>
        public void StopHook() {
            bool retKeyboard = true;

            if (hKeyboardHook != 0) {
                retKeyboard = Win32API.UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            //如果卸下钩子失败 
            if (!retKeyboard) throw new Exception("卸下钩子失败！");
        }

        public static int CopyTimes = 0;
        /// <summary>
        /// 截取全局按键，发送新按键，返回
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == (int)KeyboardKeyEvent.WM_KEYDOWN || wParam == (int)KeyboardKeyEvent.WM_SYSKEYDOWN) {
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
            return Win32API.CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            //return 0;
        }
    }
}
