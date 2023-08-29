using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using SelectAndSearch.Common.Interfaces;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace SelectAndSearch {
    public partial class PopupForm : Form, IPopupForm {
        const int WS_EX_NOACTIVATE = 0x08000000;
        //重载Form的CreateParams属性，添加不获取焦点属性值。
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }
        public PopupForm(IServiceCollection services) {
            InitializeComponent();
            //AutoScaleMode = AutoScaleMode.Dpi;
            services.AddWindowsFormsBlazorWebView();
            services.AddMasaBlazor();

            //blazorWebView1.Location = new Point(0, 0);
            //blazorWebView1.Size = this.Size;
            blazorWebView1.HostPage = "wwwroot/Popup.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<Popup>("#app");
            blazorWebView1.AutoScroll = false;
        }

        public void ShowForm(Point point) {
            this.StartPosition = FormStartPosition.Manual;//窗体其实位置类型，manual由location指定
            this.Location = point;
            this.TopMost = true;
            this.Visible = true;
        }

        
        public void HideForm() {
            if (this != null && this.Visible) {
                Rectangle rect = this.Bounds;
                Rectangle mouseRect = new Rectangle(new Point(Control.MousePosition.X, Control.MousePosition.Y), new Size(5, 5));
                if (!rect.Contains(mouseRect)) {
                    this.Visible = false;

                }

            }
        }
    }
}
