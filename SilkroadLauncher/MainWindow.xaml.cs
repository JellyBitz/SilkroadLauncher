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
            // Create viewmodel
            DataContext = new LauncherViewModel(this);
        }

        #region Events about UI behavior only
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
