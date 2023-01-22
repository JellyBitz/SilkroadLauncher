using System.Windows;
using System.Windows.Input;
namespace SilkroadLauncher
{
    public partial class MainWindow : Window, IWindow
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

        #region Interface Implementation
        public new WindowState WindowState {
            get => base.WindowState;
            set => base.WindowState = value;
        }
        public void ShowMessage(string Text, string Title)
        {
            Dispatcher.Invoke(() => {
                MessageBox.Show(this, Text, Title, MessageBoxButton.OK);
            });
        }
        public new void Close()
        {
            base.Close();
        }
        #endregion


        #region Events about UI behavior only
        /// <summary>
        /// Window fully loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Start loading updates
            if (DataContext is LauncherViewModel launcher)
                launcher.CheckUpdatesAsync();

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