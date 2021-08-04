using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace SilkroadLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Public Properties
        /// <summary>
        /// Keeps instance creation from this application
        /// </summary>
        public static Mutex Mutex { get; private set; } = null;
		#endregion

		#region Private Helpers
		protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Check the instance lock
            var thisProcess = Process.GetCurrentProcess();
            Mutex = new Mutex(true, thisProcess.MainModule.FileVersionInfo.ProductName, out bool IsFirstTime);

            if (!IsFirstTime)
            {
                // Find all process with the same description
                var processes = Process.GetProcesses();
                foreach (var p in processes)
                {
                    try
                    {
                        // Check the process and close it
                        if (thisProcess.MainModule.FileVersionInfo.ProductName == p.MainModule.FileVersionInfo.ProductName && thisProcess.Id != p.Id)
                            p.Kill();
                    }
                    catch
                    {
                        // Ignore this process, it's not from the same platform.
                    }
                }
            }
        }
		#endregion
	}
}
