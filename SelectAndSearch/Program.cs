using Ninject;

namespace SelectAndSearch {
    internal static class Program {
        
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            var ninja = NinjaBindings.ninja;
            //ninja.Load(Assembly.GetExecutingAssembly());
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(ninja.Get<PopupForm>());
        }
    }
}