using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Windows.Input;
using test_819.Views;
using Prism.Events;

namespace test_819.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public ICommand NavigateCommand { get; }
     
       
       
   

        public MainWindowViewModel(IRegionManager regionManager)
        {
           
           _regionManager= regionManager;

            NavigateCommand = new DelegateCommand<string>(ExecuteNavigate);
        }
        private void ExecuteNavigate(string viewName)
        {
           

            _regionManager.RequestNavigate("ContentRegion",viewName);
        }

        public class MessageEvent:PubSubEvent<string>
        {

        }

      




    }
}
