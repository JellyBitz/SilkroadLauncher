using Pk2ReaderAPI;
using SilkroadLauncher.Network;
using SilkroadLauncher.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
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
        private string m_Title = "Silkroad Online";
        /// <summary>
        /// The process logged in the application
        /// </summary>
        private string m_ProcessLogged = "Loading...";
        /// <summary>
        /// Pk2 reader
        /// </summary>
        private Pk2Reader m_Pk2Reader;
        /// <summary>
        /// Division info
        /// </summary>
        private Dictionary<string,List<string>> m_DivisionInfo;
        /// <summary>
        /// Gateway port
        /// </summary>
        private ushort m_Gateport;
        /// <summary>
        /// Client version
        /// </summary>
        private uint m_Version;
        /// <summary>
        /// Locale type
        /// </summary>
        private byte m_Locale;
        /// <summary>
        /// Arguments used to start the game client
        /// </summary>
        private string m_SRClientArguments;
        /// <summary>
        /// Indicates if the Pk2 has been loaded correctly
        /// </summary>
        private bool m_IsLoaded;
        /// <summary>
        /// The initial connection to server
        /// </summary>
        private Session m_GatewaySession;
        /// <summary>
        /// Indicates if the launcher is checking for updates
        /// </summary>
        private bool m_IsCheckingUpdates;
        /// <summary>
        /// Indicates if cannot connect to server
        /// </summary>
        private bool m_IsUnderInspection = true;
        /// <summary>
        /// Recent web notices
        /// </summary>
        private List<WebNoticeViewModel> m_WebNotices = new List<WebNoticeViewModel>();
        /// <summary>
        /// The notice being shown
        /// </summary>
        private WebNoticeViewModel m_SelectedWebNotice;
        /// <summary>
        /// Indicates if the launcher is on updating process
        /// </summary>
        private bool m_IsUpdating;
        /// <summary>
        /// Indicates if the game can be started
        /// </summary>
        private bool m_CanStartGame;
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
        /// <summary>
        /// Client locale
        /// </summary>
        public byte Locale
        {
            get { return m_Locale; }
            set
            {
                // set new value
                m_Locale = value;
                // notify event
                OnPropertyChanged(nameof(Locale));
            }
        }
        /// <summary>
        /// Client Version
        /// </summary>
        public uint Version
        {
            get { return m_Version; }
            set
            {
                // set new value
                m_Version = value;
                // notify event
                OnPropertyChanged(nameof(Version));
            }
        }
        /// <summary>
        /// Check if the Pk2 has been correctly loaded
        /// </summary>
        public bool IsLoaded
        {
            get { return m_IsLoaded; }
            set
            {
                // set new value
                m_IsLoaded = value;
                // notify event
                OnPropertyChanged(nameof(IsLoaded));
            }
        }
        /// <summary>
        /// Check if the launcher is looking for update the client
        /// </summary>
        public bool IsCheckingUpdates
        {
            get { return m_IsCheckingUpdates; }
            set
            {
                // set new value
                m_IsCheckingUpdates = value;
                // notify event
                OnPropertyChanged(nameof(IsCheckingUpdates));
            }
        }
        /// <summary>
        /// Check if cannot connect to server
        /// </summary>
        public bool IsUnderInspection
        {
            get { return m_IsUnderInspection; }
            set
            {
                // set new value
                m_IsUnderInspection = value;
                // notify event
                OnPropertyChanged(nameof(IsUnderInspection));
            }
        }
        /// <summary>
        /// Inspection predefined message
        /// </summary>
        public string InspectionMessage { get; } = "The server is undergoing inspection or updates.\nConnect to http://silkroadonline.net/ for more information.";
        /// <summary>
        /// All notices loaded after checking updates
        /// </summary>
        public List<WebNoticeViewModel> WebNotices
        {
            get { return m_WebNotices; }
            set
            {
                m_WebNotices = value;
                // notify event
                OnPropertyChanged(nameof(WebNotices));
            }
        }
        /// <summary>
        /// The notice selected, the first one as default.
        /// </summary>
        public WebNoticeViewModel SelectedWebNotice
        {
            get { return m_SelectedWebNotice; }
            set
            {
                // set new value
                m_SelectedWebNotice = value;
                // notify event
                OnPropertyChanged(nameof(SelectedWebNotice));
            }
        }
        /// <summary>
        /// Check if the launcher is updating the client
        /// </summary>
        public bool IsUpdating
        {
            get { return m_IsUpdating; }
            set
            {
                // set new value
                m_IsUpdating = value;
                // notify event
                OnPropertyChanged(nameof(IsUpdating));
            }
        }
        /// <summary>
        /// Check if the game can be started
        /// </summary>
        public bool CanStartGame
        {
            get { return m_CanStartGame; }
            set
            {
                // set new value
                m_CanStartGame = value;
                // notify event
                OnPropertyChanged(nameof(CanStartGame));
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
        /// <summary>
        /// Starts the client
        /// </summary>
        public ICommand CommandStartGame { get; set; }
        /// <summary>
        /// Run the register link website
        /// </summary>
        public ICommand CommandRegisterLink { get; set; }
        /// <summary>
        /// Run the guide link website
        /// </summary>
        public ICommand CommandGuideLink { get; set; }
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
            CommandClose = new RelayCommand(() => {
                m_GatewaySession?.Stop();
                m_Window.Close();
            });
            CommandStartGame = new RelayCommand(StartGame);
            CommandRegisterLink = new RelayCommand(() => RunLink("http://silkroadonline.net/"));
            CommandGuideLink = new RelayCommand(() => RunLink("https://www.google.com/"));
            #endregion

            // Load Pk2 data 
            LoadPk2();

            // Set global
            Globals.LauncherViewModel = this;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Show message to the user
        /// </summary>
        public void ShowMessage(string Text)
        {
            m_Window.Dispatcher.Invoke(() => {
                MessageBox.Show(m_Window, Text, Title, MessageBoxButton.OK);
            });
        }
        /// <summary>
        /// Check and loads the patch updates
        /// </summary>
        public async void CheckUpdatesAsync()
        {
            // Avoid connection
            if (!IsLoaded)
                return;

            IsCheckingUpdates = true;
            // Check all IP's and try to connect one at least
            m_GatewaySession = new Session();

            // Add handlers for updating
            m_GatewaySession.AddHandler(GatewayModule.Opcode.GLOBAL_IDENTIFICATION, new PacketHandler(GatewayModule.Server_GlobalIdentification));
            m_GatewaySession.AddHandler(GatewayModule.Opcode.SERVER_PATCH_RESPONSE, new PacketHandler(GatewayModule.Server_PatchResponse));
            m_GatewaySession.AddHandler(GatewayModule.Opcode.SERVER_SHARD_LIST_RESPONSE, new PacketHandler(GatewayModule.Server_ShardListResponse));
            m_GatewaySession.AddHandler(GatewayModule.Opcode.SERVER_WEB_NOTICE_RESPONSE, new PacketHandler(GatewayModule.Server_WebNoticeResponse));

            m_GatewaySession.Disconnect += new EventHandler((_Session, _Event) => {
                System.Diagnostics.Debug.WriteLine("Gateway: Session disconnected");
            });
            bool connectionSolved = false;
            // Save at the same time the connection arguments
            int divIndex = 0, hostIndex = 0;
            foreach (var division in m_DivisionInfo)
            {
                for (int i = 0; i < division.Value.Count; i++)
                {
                    // Try to connect to the address
                    System.Diagnostics.Debug.WriteLine("Starting Session..");
                    connectionSolved = await Task.Run(() => m_GatewaySession.Start(division.Value[i], m_Gateport, 5000));
                    if (connectionSolved)
                    {
                        hostIndex = i;
                        break;
                    }
                }
                if (connectionSolved)
                {
                    m_SRClientArguments = "0 \\" + m_Locale + " " + divIndex + " " + hostIndex;
                    break;
                }
                divIndex++;
            }
            // Not able to connect to server
            if (!connectionSolved)
            {
                IsCheckingUpdates = false;
                IsUnderInspection = true;
                ShowMessage(InspectionMessage);
            }
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Try to loads all required everything to create the connection to server
        /// </summary>
        private void LoadPk2()
        {
            try
            {
                // Load pk2 reader
                m_Pk2Reader = new Pk2Reader(Globals.MediaPk2FileName,Globals.BlowfishKey);

                // Extract essential stuffs for the process
                if (m_Pk2Reader.TryGetDivisionInfo(out m_DivisionInfo) 
                    && m_Pk2Reader.TryGetGateport(out m_Gateport) 
                    && m_Pk2Reader.TryGetVersion(out m_Version)
                    && m_Pk2Reader.TryGetLocale(out m_Locale))
                {
                    IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                m_Pk2Reader?.Close();
            }
        }
        private void StartGame()
        {
            if (CanStartGame && File.Exists(Globals.ClientFileName))
                System.Diagnostics.Process.Start(Globals.ClientFileName, m_SRClientArguments);
        }
        private void RunLink(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
        #endregion
    }
}
