using SelectAndSearch.Common.API;
using SelectAndSearch.Common.Enums;
using SelectAndSearch.Common.Interfaces;
using System.Diagnostics;
using static SelectAndSearch.Common.Interfaces.IHook;

namespace SelectAndSearch.Common.Hooks {
    public class MouseHook : IHook {
        public IPopupForm PopupForm { get; set; }
        public MouseHook(IPopupForm popupForm) {
            PopupForm = popupForm;
        }

        HookProc MouseHookProcedure;

        /// <summary>
        /// 启动鼠标钩子
        /// </summary>
        public void StartHook() {
            // install Mouse hook 
            if (hMouseHook == 0) {
                //实例化委托
                MouseHookProcedure = new HookProc(MouseHookProc);
                Process curProcess = Process.GetCurrentProcess();
                ProcessModule curModule = curProcess.MainModule;
                hMouseHook = Win32API.SetWindowsHookEx((int)MouseKeyEvent.WH_MOUSE_LL, MouseHookProcedure, Win32API.GetModuleHandle(curModule.ModuleName), 0);


            }

        }

        /// <summary>
        /// 停止鼠标钩子
        /// </summary>
        public void StopHook() {
            bool retKeyboard = true;
            if (hMouseHook != 0) {
                retKeyboard = Win32API.UnhookWindowsHookEx(hMouseHook);
                hMouseHook = 0;
            }
            //如果卸下钩子失败 
            if (!(retKeyboard)) throw new Exception("卸下钩子失败！");
        }

        /// <summary>
        /// 定义鼠标钩子句柄.
        /// </summary>
        int hMouseHook = 0;



        /// <summary>
        /// 截取鼠标按键
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam) {
            if (nCode >= 0) {
                switch (wParam) {
                    case (int)MouseKeyEvent.WM_LBUTTONDOWN:
                        PopupForm.HideForm();
                        break;
                }

            }
            return Win32API.CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }
    }
}
