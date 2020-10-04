
using System.Windows;

namespace SilkroadLauncher
{
    public interface IWindow
    {
        /// <summary>
        /// State of the window
        /// </summary>
        WindowState WindowState { get; set; }
        /// <summary>
        /// Show message to the user
        /// </summary>
        void ShowMessage(string Text,string Title);
        /// <summary>
        /// Close the window and all processes associated
        /// </summary>
        void Close();
    }
}
