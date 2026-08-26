// ViewModels/View2ViewModel.cs
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using test_819.Services;

namespace test_819.ViewModels
{
    public class View2ViewModel : BindableBase
    {
        private MusicPlayerService _player;
        private ObservableCollection<TrackInfo> _tracks;
        private TrackInfo _currentTrack;
        private bool _isPlaying;
        private double _volume;
        private double _progress;
        private string _currentTime;
        private string _totalTime;

        public ObservableCollection<TrackInfo> Tracks
        {
            get => _tracks;
            set => SetProperty(ref _tracks, value);
        }

        public TrackInfo CurrentTrack
        {
            get => _currentTrack;
            set => SetProperty(ref _currentTrack, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        public double Volume
        {
            get => _volume;
            set
            {
                SetProperty(ref _volume, value);
                _player.SetVolume(value / 100);
            }
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public string TotalTime
        {
            get => _totalTime;
            set => SetProperty(ref _totalTime, value);
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand SelectTrackCommand { get; }

        public View2ViewModel()
        {
            _player = MusicPlayerService.Instance;

            // 订阅事件
            _player.TrackChanged += OnTrackChanged;
            _player.ProgressUpdated += OnProgressUpdated;
            _player.PlayStateChanged += OnPlayStateChanged;

            // 初始化命令
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            NextCommand = new DelegateCommand(Next);
            PreviousCommand = new DelegateCommand(Previous);
            SelectTrackCommand = new DelegateCommand<int?>(SelectTrack);

            // 加载播放列表
            LoadPlaylist();

            // 初始化音量
            Volume = 80;
        }

        private void LoadPlaylist()
        {
            Tracks = new ObservableCollection<TrackInfo>(_player.GetPlaylist());
            CurrentTrack = _player.GetCurrentTrack();
            IsPlaying = CurrentTrack?.IsPlaying ?? false;
        }

        private void Play()
        {
            if (CurrentTrack == null)
            {
                _player.Play(0);
            }
            else
            {
                _player.Resume();
            }
            IsPlaying = true;
            UpdatePlaylistState();
        }

        private void Pause()
        {
            _player.Pause();
            IsPlaying = false;
            UpdatePlaylistState();
        }

        private void Next()
        {
            _player.Next();
            UpdatePlaylistState();
        }

        private void Previous()
        {
            _player.Previous();
            UpdatePlaylistState();
        }

        private void SelectTrack(int? index)
        {
            if (index.HasValue)
            {
                _player.Play(index.Value);
                IsPlaying = true;
                UpdatePlaylistState();
            }
        }

        private void UpdatePlaylistState()
        {
            Tracks = new ObservableCollection<TrackInfo>(_player.GetPlaylist());
            CurrentTrack = _player.GetCurrentTrack();
            IsPlaying = CurrentTrack?.IsPlaying ?? false;
        }

        private void OnTrackChanged(object sender, TrackInfo track)
        {
            UpdatePlaylistState();
        }

        private void OnProgressUpdated(object sender, double progress)
        {
            Progress = progress * 100;

            if (_player != null)
            {
                try
                {
                    var mediaPlayer = typeof(MusicPlayerService)
                        .GetField("_mediaPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.GetValue(_player) as MediaPlayer;

                    if (mediaPlayer != null && mediaPlayer.NaturalDuration.HasTimeSpan)
                    {
                        var total = mediaPlayer.NaturalDuration.TimeSpan;
                        var current = mediaPlayer.Position;
                        CurrentTime = current.ToString(@"mm\:ss");
                        TotalTime = total.ToString(@"mm\:ss");
                    }
                }
                catch { }
            }
        }

        private void OnPlayStateChanged(object sender, bool isPlaying)
        {
            IsPlaying = isPlaying;
            UpdatePlaylistState();
        }
    }
}