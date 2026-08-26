// View1ViewModel.cs
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace test_819.ViewModels
{
    public class SlideItem
    {
        public string ImagePath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class IndicatorItem
    {
        public bool IsActive { get; set; }
        public int Index { get; set; }
    }

    public class View1ViewModel : BindableBase
    {
        private ObservableCollection<SlideItem> _slides;
        public ObservableCollection<SlideItem> Slides
        {
            get => _slides;
            set => SetProperty(ref _slides, value);
        }

        private ObservableCollection<IndicatorItem> _indicators;
        public ObservableCollection<IndicatorItem> Indicators
        {
            get => _indicators;
            set => SetProperty(ref _indicators, value);
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                SetProperty(ref _currentIndex, value);
                UpdateIndicators();
            }
        }

        private bool _isPlaying = true;
        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        public View1ViewModel()
        {
            LoadSlideData();
        }

        private void LoadSlideData()
        {
            Slides = new ObservableCollection<SlideItem>
            {
                new SlideItem
                {
                    // ✅ 使用 /Images/ 路径（注意是 .png）
                    ImagePath = "D:\\Projects\\test_819\\test_819\\Images\\slide1.png",
                    Title = "欢迎来到轮播图",
                    Description = "这是一个功能完整的轮播图控件"
                },
                new SlideItem
                {
                    ImagePath = "D:\\Projects\\test_819\\test_819\\Images\\slide2.png",
                    Title = "支持手动切换",
                    Description = "点击左右箭头或指示器切换图片"
                },
                new SlideItem
                {
                    ImagePath = "D:\\Projects\\test_819\\test_819\\Images\\slide3.png",
                    Title = "自动播放功能",
                    Description = "可自动轮播，也支持暂停"
                },
                new SlideItem
                {
                    ImagePath = "D:\\Projects\\test_819\\test_819\\Images\\slider4.jpg",
                    Title = "优雅的过渡动画",
                    Description = "切换时带有平滑的缩放效果"
                },
                new SlideItem
                {
                    ImagePath = "D:\\Projects\\test_819\\test_819\\Images\\slider5.jpg",
                    Title = "完全可定制",
                    Description = "颜色、字体、动画都可根据需求调整"
                }
            };

            // 初始化指示器
            Indicators = new ObservableCollection<IndicatorItem>();
            for (int i = 0; i < Slides.Count; i++)
            {
                Indicators.Add(new IndicatorItem { Index = i, IsActive = i == 0 });
            }
            CurrentIndex = 0;
        }

        private void UpdateIndicators()
        {
            if (Indicators == null) return;

            for (int i = 0; i < Indicators.Count; i++)
            {
                Indicators[i].IsActive = (i == CurrentIndex);
            }
        }

        public void NextSlide()
        {
            if (Slides == null || Slides.Count == 0) return;
            CurrentIndex = (CurrentIndex + 1) % Slides.Count;
        }

        public void PrevSlide()
        {
            if (Slides == null || Slides.Count == 0) return;
            CurrentIndex = (CurrentIndex - 1 + Slides.Count) % Slides.Count;
        }

        public void GoToSlide(int index)
        {
            if (index >= 0 && index < Slides.Count)
            {
                CurrentIndex = index;
            }
        }
    }
}