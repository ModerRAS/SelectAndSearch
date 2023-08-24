using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.Enums {
    public enum MouseKeyEvent {
        WM_MOUSEMOVE = 0x200,
        WM_LBUTTONDOWN = 0x201,
        WM_RBUTTONDOWN = 0x204,
        WM_MBUTTONDOWN = 0x207,
        WM_LBUTTONUP = 0x202,
        WM_RBUTTONUP = 0x205,
        WM_MBUTTONUP = 0x208,
        WM_LBUTTONDBLCLK = 0x203,
        WM_RBUTTONDBLCLK = 0x206,
        WM_MBUTTONDBLCLK = 0x209,
        WH_MOUSE_LL = 14
    }
}
