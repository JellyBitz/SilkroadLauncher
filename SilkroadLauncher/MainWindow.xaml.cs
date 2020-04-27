using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace SilkroadLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                // Set viewmodel
                DataContext = new LauncherViewModel(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Environment.Exit(0);
            }
            InitializeComponent();
        }
    }
}
