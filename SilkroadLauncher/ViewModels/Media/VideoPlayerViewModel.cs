using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SilkroadLauncher
{
    public class VideoPlayerViewModel : BaseViewModel
    {
        #region Private Members
        /// <summary>
        /// Video player handling all the stuffs
        /// </summary>
        private MediaElement m_MediaPlayer;
        /// <summary>
        /// The Time line visually displaying video position
        /// </summary>
        private Slider m_SliderTimeline;
        /// <summary>
        /// The timer running on playing
        /// </summary>
        private DispatcherTimer m_TimerPlaying;
        /// <summary>
        /// Video player state
        /// </summary>
        private bool
            m_IsLoaded,
            m_IsPlaying,
            m_IsAtFinish,
            m_IsAtStart;
        #endregion

        #region Public Properties
        /// <summary>
        /// Check if player has been loaded before
        /// </summary>
        public bool IsLoaded
        {
            get { return m_IsLoaded; }
            set
            {
                m_IsLoaded = value;
                OnPropertyChanged(nameof(IsLoaded));
            }
        }
        /// <summary>
        /// Check if player is being used
        /// </summary>
        public bool IsPlaying
        {
            get { return m_IsPlaying; }
            set
            {
                m_IsPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        /// <summary>
        /// Check if player is at the start
        /// </summary>
        public bool IsAtStart {
            get { return m_IsAtStart; }
            set
            {
                m_IsAtStart = value;
                OnPropertyChanged(nameof(IsAtStart));
            }
        }
        /// <summary>
        /// Check if player is at the end
        /// </summary>
        public bool IsAtFinish
        {
            get { return m_IsAtFinish; }
            set
            {
                m_IsAtFinish = value;
                OnPropertyChanged(nameof(IsAtFinish));
            }
        }

        /// <summary>
        /// Current playing video position
        /// </summary>
        public double Position
        {
            get { return m_MediaPlayer.Position.TotalMilliseconds; }
            set
            {
                if (IsAtFinish)
                    IsAtFinish = false;
                // Set video position
                m_MediaPlayer.Position = TimeSpan.FromMilliseconds(value);
                OnPropertyChanged(nameof(Position));
            }
        }
        /// <summary>
        /// Video player volume between 0 and 1
        /// </summary>
        public double Volume {
            get { return m_MediaPlayer.Volume; }
            set {
                // Mute stuff
                if (value == 0) { 
                    IsMuted = true;
                    OnPropertyChanged(nameof(IsMuted));
                } else if (IsMuted) { 
                    IsMuted = false;
                    OnPropertyChanged(nameof(IsMuted));
                }
                // Set volume
                m_MediaPlayer.Volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }
        /// <summary>
        /// Check if player has volume on 0
        /// </summary>
        public bool IsMuted{ get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Play, Pause or Replay the video, all depending on his current player state
        /// </summary>
        public ICommand CommandSmartPlay { get; set; }
        #endregion

        #region Constructor
        public VideoPlayerViewModel(MediaElement MediaPlayer, Slider SliderTimeline)
        {
            m_MediaPlayer = MediaPlayer;
            m_SliderTimeline = SliderTimeline;
            // Requirements
            m_MediaPlayer.LoadedBehavior = MediaState.Manual;
            m_MediaPlayer.ScrubbingEnabled = true;

            // Event setup
            m_MediaPlayer.Loaded += MediaPlayer_Loaded;
            m_MediaPlayer.MediaOpened += MediaPlayer_Opened;
            m_MediaPlayer.MediaEnded += MediaPlayer_Ended;

            // Commands setup
            CommandSmartPlay = new RelayCommand(()=> {
                if (!m_MediaPlayer.IsLoaded)
                    return;
                IsAtStart = false;
                // Check if is a replay
                if (IsAtFinish)
                {
                    m_SliderTimeline.Value = 0;
                    m_MediaPlayer.Stop();
                    IsAtFinish = false;
                }
                // Do play stuffs
                if (IsPlaying)
                {
                    m_MediaPlayer.Pause();
                    m_TimerPlaying.Stop();
                    IsPlaying = false;
                }
                else
                {
                    // Start
                    m_MediaPlayer.Play();
                    IsPlaying = true;
                    // Keep the slider running
                    m_TimerPlaying = new DispatcherTimer();
                    m_TimerPlaying.Interval = TimeSpan.FromSeconds(1);
                    m_TimerPlaying.Tick += MediaPlayer_OnPlaying;
                    m_TimerPlaying.Start();
                }
            });
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the resource to a physical file from assembly for the media player
        /// </summary>
        public bool LoadResource(string ResourceName,string ResourceOutputPath)
        {
            // Test resource
            if (string.IsNullOrEmpty(ResourceName) || string.IsNullOrEmpty(ResourceOutputPath))
                return false;
            byte[] buffer;
            try
            {
                buffer = (byte[])Properties.Resources.ResourceManager.GetObject(ResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
            // Create Resource physically on the current directory
            var myDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            ResourceOutputPath = Path.GetFullPath(Path.Combine(myDir, ResourceOutputPath));
            var outDir = Path.GetDirectoryName(ResourceOutputPath);
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            File.WriteAllBytes(ResourceOutputPath, buffer);
            // Set video path
            m_MediaPlayer.Source = new Uri(ResourceOutputPath, UriKind.Absolute);
            return true;
        }
        #endregion

        #region Events
        private void MediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            // Force silent loading to the first frame
            m_MediaPlayer.Volume = 0;
            m_MediaPlayer.Play();
            m_MediaPlayer.Pause();
            m_MediaPlayer.Position = TimeSpan.FromMilliseconds(1);
            m_MediaPlayer.Volume = Volume;
            IsAtStart = true;
        }
        private void MediaPlayer_Opened(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
            // Set initial values
            Volume = 1;
            // Set the maximum
            m_SliderTimeline.Maximum = m_MediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            m_SliderTimeline.Value = 0;
        }
        private void MediaPlayer_Ended(object sender, RoutedEventArgs e)
        {
            // Stop updating timeline slider
            m_TimerPlaying.Stop();
            // Update player state
            m_MediaPlayer.Pause();
            IsPlaying = false;
            IsAtFinish = true;
            // Set to maximum the timeslider
            m_SliderTimeline.Value = m_MediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
        }
        private void MediaPlayer_OnPlaying(object sender, EventArgs e)
        {
            // Updates slider to the current time being played
            m_SliderTimeline.Value = m_MediaPlayer.Position.TotalMilliseconds;
        }
        #endregion
    }
}
