using Pk2ReaderAPI;
using SilkroadLauncher.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SilkroadLauncher
{
    public class LauncherViewModel : BaseViewModel
    {
        #region Private Properties
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window m_Window;
        /// <summary>
        /// The title of the application
        /// </summary>
        private string m_Title = "Launcher - Silkroad Latino";
        /// <summary>
        /// The process logged in the application
        /// </summary>
        private string m_ProcessLogged = "Loading...";
        /// <summary>
        /// Key used to decrypt the pk2
        /// </summary>
        private string m_BlowfishKey = "169841";
        /// <summary>
        /// Pk2 reader
        /// </summary>
        private Pk2Reader m_Pk2Reader;
        /// <summary>
        /// Division info
        /// </summary>
        private Dictionary<string,List<string>> m_DivisionInfo;
        #endregion


        #region Public Properties
        /// <summary>
        /// Title of the application
        /// </summary>
        public string Title
        {
            get
            {
                return m_Title;
            }
            set
            {
                m_Title = value;
                // notify event
                OnPropertyChanged(nameof(Title));
            }
        }
        /// <summary>
        /// Process logged being executed by the application
        /// </summary>
        public string ProcessLogged
        {
            get { return m_ProcessLogged; }
            set
            {
                if (m_ProcessLogged == value)
                    return;
                // set new value
                m_ProcessLogged = value;
                // notify event
                OnPropertyChanged(nameof(ProcessLogged));
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Minimize the window
        /// </summary>
        public ICommand CommandMinimize { get; set; }
        /// <summary>
        /// Switch the window between restore and maximize
        /// </summary>
        public ICommand CommandRestore { get; set; }
        /// <summary>
        /// Close the window
        /// </summary>
        public ICommand CommandClose { get; set; }
        #endregion
        
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public LauncherViewModel(Window Window)
        {
            // Save reference
            m_Window = Window;
            
            #region Commands Setup
            // Windows commands
            CommandMinimize = new RelayCommand(() => m_Window.WindowState = WindowState.Minimized);
            CommandRestore = new RelayCommand(() =>
            {
                // Check the WindowState and change it
                m_Window.WindowState = m_Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
            CommandClose = new RelayCommand(m_Window.Close);
            #endregion
            
            OnLoad();
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Loads everything to check/create a connection to server
        /// </summary>
        private void OnLoad()
        {
            // Load pk2 reader
            m_Pk2Reader = new Pk2Reader("media.pk2", m_BlowfishKey);

            // Get IP's to check connection
            if (m_Pk2Reader.TryGetDivisionInfo(out m_DivisionInfo))
            {

            }
        }
        #endregion
    }
}
