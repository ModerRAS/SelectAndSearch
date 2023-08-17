using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectAndSearch {
    public partial class PopupForm : Form {
        const int WS_EX_NOACTIVATE = 0x08000000;
        //重载Form的CreateParams属性，添加不获取焦点属性值。
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }
        public PopupForm() {
            InitializeComponent();
        }
    }
}
