using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
namespace SilkroadLauncher
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Set viewmodel
            LauncherViewModel.Instance.SetWindow(this);
            DataContext = LauncherViewModel.Instance;
        }

        #region Events about UI behavior only
        /// <summary>
        /// Window fully loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Start loading updates
            //if (DataContext is LauncherViewModel launcher)
            //    launcher.CheckUpdatesAsync();

            // Force to the top
            Topmost = true;
            Topmost = false;
        }
        /// <summary>
        /// Drag the window when the control is click holding
        /// </summary>
        private void Control_MouseLeftButtonDown_DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion
    }
}