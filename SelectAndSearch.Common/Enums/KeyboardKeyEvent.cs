using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.Enums {
    public enum KeyboardKeyEvent {
        /// <summary>
        /// 普通按键消息
        /// </summary>
        WM_KEYDOWN = 0x100,
        /// <summary>
        /// 系统按键消息
        /// </summary>
        WM_SYSKEYDOWN = 0x104,
        WH_KEYBOARD_LL = 13
    }
}
