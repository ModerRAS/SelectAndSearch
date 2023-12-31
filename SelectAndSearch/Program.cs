using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SelectAndSearch.Common.API;
using SelectAndSearch.Common.Hooks;
using SelectAndSearch.Common.Interfaces;
using SelectAndSearch.Common.Managers;
using SelectAndSearch.Common.Models;
using SelectAndSearch.Common.OCR.PaddleOCR;
using SelectAndSearch.Common.OCR.RapidOCR;
using SelectAndSearch.Common.Services;

namespace SelectAndSearch {
    internal static class Program {
        public static IHost host { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            host = Host.CreateDefaultBuilder()
                .ConfigureServices(service => {
                    service.AddSingleton<ExcelManager>();
                    service.AddSingleton<LuceneManager>();
                    service.AddTransient<ImportService>();
                    service.AddTransient<SearchService>();
                    service.AddSingleton<MainForm>();
                    service.AddSingleton<KeyCaptureDialog>();
                    service.AddSingleton<IPopupForm,PopupForm>();
                    //service.AddSingleton<Popup>();
                    service.AddSingleton<ClipboardHook>();
                    service.AddSingleton<MouseHook>();
                    service.AddSingleton<OCRHook>();
                    service.AddSingleton<GlobalConfig>();
                    if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1) {
                        service.AddSingleton<IOCR, RapidOCR>();
                    } else {
                        service.AddSingleton<IOCR, PaddleOCR>();
                    }
                    service.AddSingleton(service);
#if DEBUG
                    service.AddBlazorWebViewDeveloperTools();
#endif
                })
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(options => {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "[yyyy/MM/dd HH:mm:ss] ";
                    });
#if DEBUG
                    logging.AddDebug();
#endif
                }).Build();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(host.Services.GetService<MainForm>());
        }
    }
}