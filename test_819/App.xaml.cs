
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using test_819.Views;

namespace test_819
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
         
        }

       
    }
}
