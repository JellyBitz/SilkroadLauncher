using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SilkroadLauncher.Controls
{
    public partial class VideoBackground : UserControl
    {
        #region Constructor
        public VideoBackground()
        {
            InitializeComponent();
            InitiaizeControls();
        }
        private void InitiaizeControls()
        {
            MediaVideo.LoadedBehavior = MediaState.Manual;
            // For auto thumbnail
            MediaVideo.ScrubbingEnabled = true;
            // Playing behavior
            MediaVideo.Loaded += new RoutedEventHandler((s,e) => {
                MediaVideo.Play();
            });
            MediaVideo.MediaOpened += new RoutedEventHandler((s, e) => {
                MediaVideo.Play();
            });
            MediaVideo.MediaEnded += new RoutedEventHandler((s,e) => {
                if (RepeatBehavior) {
                    // Reset and play
                    MediaVideo.Position = TimeSpan.Zero;
                    MediaVideo.Play();
                }
            });
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Resource name
        /// </summary>
        public string ResourceName
        {
            get { return (string)GetValue(ResourceNameProperty); }
            set { SetValue(ResourceNameProperty, value); }
        }
        /// <summary>
        /// Resource output path
        /// </summary>
        public string ResourceOutputPath
        {
            get { return (string)GetValue(ResourceOutputPathProperty); }
            set { SetValue(ResourceOutputPathProperty, value); }
        }
        public static readonly DependencyProperty ResourceNameProperty = DependencyProperty.Register("ResourceName", typeof(string), typeof(VideoBackground), new PropertyMetadata(null, OnResourceChanged));
        public static readonly DependencyProperty ResourceOutputPathProperty = DependencyProperty.Register("ResourceOutputPath", typeof(string), typeof(VideoBackground), new PropertyMetadata(null, OnResourceChanged));

        private static void OnResourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VideoBackground _this)
            {
                // Check valid resource
                if (string.IsNullOrEmpty(_this.ResourceName) || string.IsNullOrEmpty(_this.ResourceOutputPath))
                    return;
                // Check resources existence and extract buffer as byte array
                byte[] buffer = (byte[])Properties.Resources.ResourceManager.GetObject(_this.ResourceName);
                if (buffer == null) return;
                // Create directory
                var myDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                var resourcePath = Path.GetFullPath(Path.Combine(myDir, _this.ResourceOutputPath));
                // Check if resource exist already
                bool fileExist = false;
                if (File.Exists(resourcePath))
                {
                    using (StreamReader sr = new StreamReader(resourcePath))
                    {
                        // If the byte size are the same, the most PROBABLY it's the same file
                        fileExist = sr.BaseStream.Length == buffer.LongLength;
                    }
                }
                if (!fileExist)
                {
                    // Create the resource
                    var outDir = Path.GetDirectoryName(resourcePath);
                    if (!Directory.Exists(outDir))
                        Directory.CreateDirectory(outDir);
                    File.WriteAllBytes(resourcePath, buffer);
                }
                // Set video path
                _this.MediaVideo.Source = new Uri(resourcePath, UriKind.Absolute);
            }
        }
        /// <summary>
        /// Set the loop behavior of this control
        /// </summary>
        public bool RepeatBehavior
        {
            get { return (bool)GetValue(RepeatBehaviorProperty); }
            set { SetValue(RepeatBehaviorProperty, value); }
        }
        public static readonly DependencyProperty RepeatBehaviorProperty = DependencyProperty.Register("RepeatBehavior", typeof(bool), typeof(VideoBackground), new PropertyMetadata());
        /// <summary>
        /// Background sound activation
        /// </summary>
        public bool Sound
        {
            get { return (bool)GetValue(SoundProperty); }
            set { SetValue(SoundProperty, value); }
        }
        public static readonly DependencyProperty SoundProperty = DependencyProperty.Register("Sound", typeof(bool), typeof(VideoBackground), new PropertyMetadata(true, OnSoundChanged));

        private static void OnSoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VideoBackground _this)
            {
                if (_this.Sound)
                    _this.MediaVideo.Volume = 1.0;
                else
                    _this.MediaVideo.Volume = 0.0;
            }
        }
        /// <summary>
        /// Background playing behavior
        /// </summary>
        public bool PlayBehavior
        {
            get { return (bool)GetValue(PlayBehaviorProperty); }
            set { SetValue(PlayBehaviorProperty, value); }
        }
        public static readonly DependencyProperty PlayBehaviorProperty = DependencyProperty.Register("PlayBehavior", typeof(bool), typeof(VideoBackground), new PropertyMetadata(true, OnPlayBehaviorChanged));
        private static void OnPlayBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VideoBackground _this)
            {
                if (_this.MediaVideo.IsLoaded)
                {
                    if (_this.PlayBehavior)
                    {
                        // Repeat
                        _this.MediaVideo.Position = TimeSpan.Zero;
                        _this.MediaVideo.Play();
                    }
                    else
                    {
                        _this.MediaVideo.Pause();
                    }
                }
            }
        }
        #endregion
    }
}