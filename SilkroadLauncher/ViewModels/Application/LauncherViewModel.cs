using Pk2ReaderAPI;
using SilkroadLauncher.Network;
using SilkroadLauncher.SilkroadCommon;
using SilkroadLauncher.Utility;
using System;
using System.Collections.Generic;
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
        /// Indicates if the user is viewing game config
        /// </summary>
        private bool m_IsViewingConfig;
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
        /// The total bytes required download to apply patch
        /// </summary>
        private ulong m_UpdatingBytesMaxDownloading;
        /// <summary>
        /// The current bytes downloaded to apply patch
        /// </summary>
        private ulong m_UpdatingBytesDownloading;
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
        /// The basic view config used by the game client
        /// </summary>
        public ConfigViewModel Config { get; private set; }
        /// <summary>
        /// Check if the launcher is on game config screen
        /// </summary>
        public bool IsViewingConfig
        {
            get { return m_IsViewingConfig; }
            set
            {
                // set new value
                m_IsViewingConfig = value;
                // notify event
                OnPropertyChanged(nameof(IsViewingConfig));
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
        /// Get the current percentage from patch download
        /// </summary>
        public int UpdatingPercentage
        {
            get {
                if (m_UpdatingBytesMaxDownloading == 0)
                    return 0;
                return (int)(m_UpdatingBytesDownloading * 100ul /m_UpdatingBytesMaxDownloading); }
        }
        /// <summary>
        /// Get or sets the max. bytes quantity to be downloaded to apply patch
        /// </summary>
        public ulong UpdatingBytesMaxDownloading
        {
            get { return m_UpdatingBytesMaxDownloading; }
            set
            {
                // set new value
                m_UpdatingBytesMaxDownloading = value;
                // notify event
                OnPropertyChanged(nameof(UpdatingBytesMaxDownloading));
            }
        }
        /// <summary>
        /// Get or sets the bytes quantity downloaded to apply patch
        /// </summary>
        public ulong UpdatingBytesDownloading
        {
            get { return m_UpdatingBytesDownloading; }
            set
            {
                // set new value
                m_UpdatingBytesDownloading = value;
                // notify event
                OnPropertyChanged(nameof(UpdatingBytesDownloading));
                OnPropertyChanged(nameof(UpdatingPercentage));
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
        /// Open an hyperlink on default browser
        /// </summary>
        public ICommand CommandOpenLink { get; set; }
        /// <summary>
        /// Set the config viewing state
        /// </summary>
        public ICommand CommandToggleConfig { get; set; }
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
            CommandOpenLink = new RelayParameterizedCommand(OpenLink);
            CommandToggleConfig = new RelayCommand(() => { IsViewingConfig = !IsViewingConfig; });
            #endregion

            // Load Pk2 data 
            LoadPk2();
            // 
            LoadConfig();

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
                    connectionSolved = await Task.Run(() => m_GatewaySession.Start(division.Value[i], m_Gateport, 10000));
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
        /// <summary>
        /// Try to loads the config or creates a new one
        /// </summary>
        private void LoadConfig()
        {
            var temp = new ConfigViewModel();
            if (temp.Load("SilkCfg.dat"))
            {
                Config = temp;
            }
            else
            {
                this.Config = new ConfigViewModel();
                Config.Save("SilkCfg.dat");
            }
        }
        /// <summary>
        /// Starts the game client if is ready
        /// </summary>
        private void StartGame()
        {
            if (CanStartGame && File.Exists(Globals.ClientFileName))
                System.Diagnostics.Process.Start(Globals.ClientFileName, m_SRClientArguments);
        }
        /// <summary>
        /// Open a link using default browser
        /// </summary>
        private void OpenLink(object url)
        {
            if(url is string link)
                System.Diagnostics.Process.Start(link);
        }
        #endregion
    }
}
