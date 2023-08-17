using SelectAndSearch.Hooks;
using SelectAndSearch.Managers;
using SelectAndSearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndSearch {
    internal class NinjaBindings : Ninject.Modules.NinjectModule {
        public override void Load() {
            Bind<LuceneManager>().To<LuceneManager>();
            Bind<SearchService>().To<SearchService>();
            Bind<MainForm>().To<MainForm>();
            Bind<PopupForm>().To<PopupForm>();
        }
    }
}
