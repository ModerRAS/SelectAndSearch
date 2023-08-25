using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.Models {
    //声明键盘钩子的封送结构类型 
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardHookStruct {
        public int vkCode; //表示一个在1到254间的虚似键盘码 
        public int scanCode; //表示硬件扫描码 
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
}
