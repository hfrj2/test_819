using System.Windows;
using System.Windows.Controls;
using test_819.Services;

namespace test_819.Views
{
    public partial class View2 : UserControl
    {
        private bool _isDragging = false;

        public View2()
        {
            InitializeComponent();
        }

        private void TrackItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is TrackInfo track)
            {
                var viewModel = DataContext as ViewModels.View2ViewModel;
                viewModel?.SelectTrackCommand?.Execute(track.Index);
            }
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _isDragging = false;
            var viewModel = DataContext as ViewModels.View2ViewModel;
            if (viewModel != null)
            {
                MusicPlayerService.Instance.SetPosition(viewModel.Progress / 100);
            }
        }
    }
}