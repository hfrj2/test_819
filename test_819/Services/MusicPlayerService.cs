// Services/MusicPlayerService.cs
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;

namespace test_819.Services
{
    public class MusicPlayerService
    {
        private static MusicPlayerService _instance;
        private MediaPlayer _mediaPlayer;
        private DispatcherTimer _progressTimer;
        private List<string> _playlist;
        private int _currentIndex = -1;
        private bool _isPlaying = false;

        // 播放状态事件
        public event EventHandler<TrackInfo> TrackChanged;
        public event EventHandler<double> ProgressUpdated;
        public event EventHandler<bool> PlayStateChanged;

        public static MusicPlayerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MusicPlayerService();
                }
                return _instance;
            }
        }

        private MusicPlayerService()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;

            _progressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _progressTimer.Tick += ProgressTimer_Tick;

            // 初始化播放列表
            _playlist = new List<string>
            {
                "D:\\Projects\\test_819\\test_819\\Audio\\GEM_ONLY.mp3",
                "D:\\Projects\\test_819\\test_819\\Audio\\song1.mp3",
                "D:\\Projects\\test_819\\test_819\\Audio\\song1.mp3",
                "D:\\Projects\\test_819\\test_819\\Audio\\song1.mp3",
                "D:\\Projects\\test_819\\test_819\\Audio\\song1.mp3"
            };
        }

        public List<TrackInfo> GetPlaylist()
        {
            var tracks = new List<TrackInfo>();
            var trackNames = new[]
            {
                "GEM - ONLY",
                "歌曲2 - 歌手B",
                "歌曲3 - 歌手C",
                "歌曲4 - 歌手D",
                "歌曲5 - 歌手E"
            };

            for (int i = 0; i < _playlist.Count; i++)
            {
                tracks.Add(new TrackInfo
                {
                    Index = i,
                    Name = trackNames[i % trackNames.Length],
                    Path = _playlist[i],
                    IsCurrent = (i == _currentIndex),
                    IsPlaying = (i == _currentIndex && _isPlaying)
                });
            }
            return tracks;
        }

        public void Play(int index)
        {
            if (index < 0 || index >= _playlist.Count) return;

            _currentIndex = index;
            _mediaPlayer.Open(new Uri(_playlist[index], UriKind.RelativeOrAbsolute));
            _mediaPlayer.Play();
            _isPlaying = true;
            _progressTimer.Start();

            PlayStateChanged?.Invoke(this, true);
            TrackChanged?.Invoke(this, new TrackInfo
            {
                Index = index,
                Name = GetTrackName(index),
                Path = _playlist[index],
                IsCurrent = true,
                IsPlaying = true
            });
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
            _isPlaying = false;
            _progressTimer.Stop();
            PlayStateChanged?.Invoke(this, false);
        }

        public void Resume()
        {
            _mediaPlayer.Play();
            _isPlaying = true;
            _progressTimer.Start();
            PlayStateChanged?.Invoke(this, true);
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
            _isPlaying = false;
            _progressTimer.Stop();
            PlayStateChanged?.Invoke(this, false);
        }

        public void Next()
        {
            if (_playlist.Count == 0) return;
            int nextIndex = (_currentIndex + 1) % _playlist.Count;
            Play(nextIndex);
        }

        public void Previous()
        {
            if (_playlist.Count == 0) return;
            int prevIndex = (_currentIndex - 1 + _playlist.Count) % _playlist.Count;
            Play(prevIndex);
        }

        public void SetVolume(double volume)
        {
            _mediaPlayer.Volume = Math.Max(0, Math.Min(1, volume));
        }

        public void SetPosition(double position)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                var duration = _mediaPlayer.NaturalDuration.TimeSpan;
                var newPosition = TimeSpan.FromSeconds(position * duration.TotalSeconds);
                _mediaPlayer.Position = newPosition;
            }
        }

        public TrackInfo GetCurrentTrack()
        {
            if (_currentIndex < 0 || _currentIndex >= _playlist.Count)
                return null;

            return new TrackInfo
            {
                Index = _currentIndex,
                Name = GetTrackName(_currentIndex),
                Path = _playlist[_currentIndex],
                IsCurrent = true,
                IsPlaying = _isPlaying
            };
        }

        private string GetTrackName(int index)
        {
            var names = new[] { "GEM - ONLY", "歌曲2 - 歌手B", "歌曲3 - 歌手C", "歌曲4 - 歌手D", "歌曲5 - 歌手E" };
            return names[index % names.Length];
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                var duration = _mediaPlayer.NaturalDuration.TimeSpan;
                ProgressUpdated?.Invoke(this, 0);
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            Next();
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"播放失败: {e.ErrorException.Message}");
            Next();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan && _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                var progress = _mediaPlayer.Position.TotalSeconds / _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressUpdated?.Invoke(this, progress);
            }
        }
    }

    public class TrackInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsPlaying { get; set; }
    }
}