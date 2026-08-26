using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace test_819.Views
{
    public partial class View1 : UserControl
    {
        private DispatcherTimer _timer;
        private ViewModels.View1ViewModel _viewModel;
        private const int SlideInterval = 4000;

        public View1()
        {
            InitializeComponent();
            Loaded += View1_Loaded;
            Unloaded += View1_Unloaded;

            PrevButton.Click += (s, e) => GoToPreviousSlide();
            NextButton.Click += (s, e) => GoToNextSlide();
        }

        private void View1_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as ViewModels.View1ViewModel;
            if (_viewModel != null)
            {
                UpdateSlideDisplay(_viewModel.CurrentIndex);
                StartAutoPlay();
            }
        }

        private void View1_Unloaded(object sender, RoutedEventArgs e)
        {
            StopAutoPlay();
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        private void StartAutoPlay()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(SlideInterval)
                };
                _timer.Tick += Timer_Tick;
            }
            _timer.Start();
            UpdatePlayPauseIcon(true);
        }

        private void StopAutoPlay()
        {
            _timer?.Stop();
            UpdatePlayPauseIcon(false);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_viewModel != null && _viewModel.IsPlaying)
            {
                GoToNextSlide();
            }
        }

        private void GoToNextSlide()
        {
            if (_viewModel == null) return;
            _viewModel.NextSlide();
            UpdateSlideDisplay(_viewModel.CurrentIndex);
        }

        private void GoToPreviousSlide()
        {
            if (_viewModel == null) return;
            _viewModel.PrevSlide();
            UpdateSlideDisplay(_viewModel.CurrentIndex);
        }

        private void UpdateSlideDisplay(int index)
        {
            if (_viewModel == null || _viewModel.Slides == null) return;

            var slide = _viewModel.Slides[index];
            if (slide != null)
            {
                // ✅ 加载图片
                LoadImage(slide.ImagePath);

                // 更新文字
                SlideTitle.Text = slide.Title;
                SlideDescription.Text = slide.Description;
                SlideProgress.Text = $"{index + 1} / {_viewModel.Slides.Count}";

                // 播放切换动画
                PlayTransitionAnimation();

                // 更新指示器
                if (_viewModel.Indicators != null)
                {
                    for (int i = 0; i < _viewModel.Indicators.Count; i++)
                    {
                        _viewModel.Indicators[i].IsActive = (i == index);
                    }
                }
            }
        }

        // ✅ 加载图片的方法
        private void LoadImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                SlideImage.Source = null;
                return;
            }

            try
            {
                // 创建 BitmapImage
                var uri = new Uri(imagePath, UriKind.Relative);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // 如果图片可以冻结，冻结以提高性能
                if (bitmap.CanFreeze)
                {
                    bitmap.Freeze();
                }

                SlideImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                // 加载失败，显示调试信息
                System.Diagnostics.Debug.WriteLine($"图片加载失败: {imagePath}, 错误: {ex.Message}");
                SlideImage.Source = null;
            }
        }

        private void PlayTransitionAnimation()
        {
            var storyboard = new Storyboard();

            var fadeOut = new DoubleAnimation(1.0, 0.5, TimeSpan.FromMilliseconds(200));
            Storyboard.SetTarget(fadeOut, SlideImage);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));
            storyboard.Children.Add(fadeOut);

            var fadeIn = new DoubleAnimation(0.5, 1.0, TimeSpan.FromMilliseconds(200))
            {
                BeginTime = TimeSpan.FromMilliseconds(250)
            };
            Storyboard.SetTarget(fadeIn, SlideImage);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
            storyboard.Children.Add(fadeIn);

            storyboard.Begin();
        }

        private void UpdatePlayPauseIcon(bool isPlaying)
        {
            if (PlayPauseIcon != null)
            {
                PlayPauseIcon.Text = isPlaying ? "\uf04c" : "\uf04b";
            }
            if (_viewModel != null)
            {
                _viewModel.IsPlaying = isPlaying;
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            _viewModel.IsPlaying = !_viewModel.IsPlaying;
            if (_viewModel.IsPlaying)
            {
                StartAutoPlay();
            }
            else
            {
                StopAutoPlay();
            }
        }

        private void Indicator_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Shapes.Ellipse ellipse &&
                ellipse.DataContext is ViewModels.IndicatorItem indicator)
            {
                StopAutoPlay();
                _viewModel?.GoToSlide(indicator.Index);
                UpdateSlideDisplay(indicator.Index);
                StartAutoPlay();
            }
        }

        private void PrevIndicatorButton_Click(object sender, RoutedEventArgs e)
        {
            StopAutoPlay();
            GoToPreviousSlide();
            StartAutoPlay();
        }

        private void NextIndicatorButton_Click(object sender, RoutedEventArgs e)
        {
            StopAutoPlay();
            GoToNextSlide();
            StartAutoPlay();
        }
    }
}