using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using static test_819.ViewModels.MainWindowViewModel;

namespace test_819.ViewModels
{
    public class ViewBViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private ObservableCollection<string> _messages;
        public ObservableCollection<string> Messages
        {
            get => _messages;
            set=>SetProperty(ref _messages, value);
        }

      
        public ViewBViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
           Messages= new ObservableCollection<string>();

            _eventAggregator.GetEvent<MessageEvent>().Subscribe(OnMessageReceived, ThreadOption.UIThread);
        }

  private void OnMessageReceived(string message)
        {
            Messages.Add($"收到:{message}(时间：{DateTime.Now:hh:mm:ss})");
        }
        public void Dispose()
        {
            _eventAggregator.GetEvent<MessageEvent>().Unsubscribe(OnMessageReceived);
        }
    }
}
