using Ninject;
using SelectAndSearch.Common.Hooks;
using SelectAndSearch.Common.Managers;
using SelectAndSearch.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch {
    internal class NinjaBindings : Ninject.Modules.NinjectModule {
        public static StandardKernel ninja { get; set; } = new StandardKernel(new NinjaBindings());
        public override void Load() {
            Bind<LuceneManager>().To<LuceneManager>();
            Bind<SearchService>().To<SearchService>();
            Bind<MainForm>().To<MainForm>();
            Bind<PopupForm>().To<PopupForm>();
            Bind<HighlightManager>().To<HighlightManager>();
            Bind<ClipboardHook>().To<ClipboardHook>();
            Bind<Popup>().To<Popup>();
        }
    }
}
