using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectAndSearch.Common.Interfaces {
    public interface IPopupForm {
        public void ShowForm(Point point);
        public void HideForm();
    }
}
