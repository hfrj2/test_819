using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace test_819.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public ICommand NavigateCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(ExecuteNavigate);
        }
        private void ExecuteNavigate(string viewName)
        {
            if (!string.IsNullOrWhiteSpace(viewName))
            {


                _regionManager.RequestNavigate("ContentRegion", viewName);
            }
        }

    }
}