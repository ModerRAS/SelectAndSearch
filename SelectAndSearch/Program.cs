using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ninject;
using SelectAndSearch.Managers;
using SelectAndSearch.Services;

namespace SelectAndSearch {
    internal static class Program {
        
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(service => {
                    service.AddSingleton<LuceneManager>();
                    service.AddTransient<SearchService>();
                    service.AddSingleton<MainForm>();
                    service.AddSingleton<PopupForm>();
                    service.AddSingleton<Popup>();
                    service.AddSingleton(service);
                })
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(options => {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "[yyyy/MM/dd HH:mm:ss] ";
                    });
                }).Build();
            var ninja = NinjaBindings.ninja;
            //ninja.Load(Assembly.GetExecutingAssembly());
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(host.Services.GetService<MainForm>());
        }
    }
}