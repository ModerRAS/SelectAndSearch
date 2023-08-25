using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch.Common.Models {
    //Declare wrapper managed POINT class.
    [StructLayout(LayoutKind.Sequential)]
    public class POINT {
        public int x;
        public int y;
    }
}
