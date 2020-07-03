using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SilkroadLauncher.Controls
{
    public partial class VideoPlayerControl : UserControl
    {
        #region Private Members
        /// <summary>
        /// The view model handling all this stuffs
        /// </summary>
        VideoPlayerViewModel m_ViewModel;
        #endregion

        #region Constructor
        public VideoPlayerControl()
        {
            InitializeComponent();
            
            // Set view model
            m_ViewModel = new VideoPlayerViewModel(MediaPlayer, SliderTimeline);
            DataContext = m_ViewModel;
            // Manual handlers
            SliderTimeline.AddHandler(MouseLeftButtonUpEvent,new System.Windows.Input.MouseButtonEventHandler((sender,e) => {
                m_ViewModel.Position = SliderTimeline.Value;
            }),true);
            SliderVolume.AddHandler(MouseLeftButtonUpEvent, new System.Windows.Input.MouseButtonEventHandler((sender, e) => {
                m_ViewModel.Volume = SliderVolume.Value;
            }),true);
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
        public static readonly DependencyProperty ResourceNameProperty = DependencyProperty.Register("ResourceName", typeof(string), typeof(VideoPlayerControl), new PropertyMetadata(null, OnResourceChanged));
        public static readonly DependencyProperty ResourceOutputPathProperty = DependencyProperty.Register("ResourceOutputPath", typeof(string), typeof(VideoPlayerControl), new PropertyMetadata(null, OnResourceChanged));
        
        private static void OnResourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VideoPlayerControl _this)
                _this.m_ViewModel.LoadResource(_this.ResourceName, _this.ResourceOutputPath);
        }
        #endregion
    }
}
