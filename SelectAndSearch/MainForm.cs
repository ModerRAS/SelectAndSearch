using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace SelectAndSearch {
    public partial class MainForm : Form {
        public MainForm(IServiceCollection services) {
            InitializeComponent();
            
            services.AddWindowsFormsBlazorWebView();
            services.AddMasaBlazor();


            blazorWebView1.HostPage = "wwwroot/index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>("#app");
        }
    }
}