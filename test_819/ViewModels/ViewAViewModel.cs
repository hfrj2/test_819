using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using static test_819.ViewModels.MainWindowViewModel;

namespace test_819.ViewModels
{
    public class ViewAViewModel : BindableBase
    {


        private readonly IEventAggregator _eventAggregator;
        private string _inputMessage;


        public string InputMessage
        {
            get => _inputMessage;
            set => SetProperty(ref _inputMessage, value);
        }
        public ViewAViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SendCommand = new DelegateCommand(ExecuteSend);
        }

        public DelegateCommand SendCommand { get; }

        private void ExecuteSend()
        {
            _eventAggregator.GetEvent<MessageEvent>().Publish(InputMessage);
        }
    }
}
