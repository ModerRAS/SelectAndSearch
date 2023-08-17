using Ninject;

namespace SelectAndSearch {
    internal static class Program {
        public static StandardKernel ninja { get; set; }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            ninja = new StandardKernel(new NinjaBindings());
            //ninja.Load(Assembly.GetExecutingAssembly());
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(ninja.Get<MainForm>());
        }
    }
}