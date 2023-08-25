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
    public partial class KeyCaptureDialog : Form {
        public Keys CapturedKey { get; private set; }

        public KeyCaptureDialog() {
            InitializeComponent();
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            CapturedKey = e.KeyCode | e.Modifiers;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
