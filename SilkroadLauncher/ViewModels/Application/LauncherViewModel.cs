using SilkroadLauncher.Network;
using SilkroadLauncher.Utility;
using SRO.PK2API;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace SilkroadLauncher
{
    /// <summary>
    /// Handles all Silkroad launcher processes
    /// </summary>
    public class LauncherViewModel : BaseViewModel
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

        #region Singleton
        /// <summary>
        /// Unique instance of this class
        /// </summary>
        public static LauncherViewModel Instance { get; } = new LauncherViewModel();
        #endregion

        #region Private Properties
        /// <summary>
        /// Application title
        /// </summary>
        private string m_Title = LauncherSettings.APP_TITLE;
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private IWindow m_Window;
        /// <summary>
        /// Division info
        /// </summary>
        private Dictionary<string,List<string>> m_DivisionInfo;
        /// <summary>
        /// Gateway port
        /// </summary>
        private ushort m_Gateport;
        /// <summary>
        /// Client version handled internally by game
        /// </summary>
        private uint m_Version;
        /// <summary>
        /// Client version shown to the user
        /// </summary>
        private string m_ClientVersion = string.Empty;
        /// <summary>
        /// Locale type
        /// </summary>
        private byte m_Locale;
        /// <summary>
        /// Arguments used to start the game client
        /// </summary>
        private string m_ClientArgs;
        /// <summary>
        /// Indicates if the Pk2 has been loaded correctly
        /// </summary>
        private bool m_IsLoaded;
        /// <summary>
        /// Indicates if the user is viewing game config
        /// </summary>
        private bool m_IsViewingConfig;
        /// <summary>
        /// Game bsic config
        /// </summary>
        private ConfigViewModel m_Config = new ConfigViewModel();
        /// <summary>
        /// Indicates if the user is viewing language options
        /// </summary>
        private bool m_IsViewingLangConfig;
        /// <summary>
        /// Language temporally being selected as language option
        /// </summary>
        private int m_LangConfigIndex;
        /// <summary>
        /// The initial connection to server
        /// </summary>
        private Client m_GatewaySession;
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
        /// The current patch percentage
        /// </summary>
        private int m_UpdatingPercentage;
        /// <summary>
        /// The current file path being updated
        /// </summary>
        private string m_UpdatingFilePath;
        /// <summary>
        /// The current file progress updating percentage
        /// </summary>
        private int m_UpdatingFilePercentage;
        /// <summary>
        /// Indicates if the game can be started
        /// </summary>
        private bool m_CanStartGame;
        #endregion

        #region Public Properties
        /// <summary>
        /// Application title shown on windows title bar
        /// </summary>
        public string Title
        {
            get => m_Title;
            set
            {
                m_Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        /// <summary>
        /// Client locale
        /// </summary>
        public byte Locale
        {
            get => m_Locale;
            set
            {
                // set new value
                m_Locale = value;
                // notify event
                OnPropertyChanged(nameof(Locale));
            }
        }
        /// <summary>
        /// Client version handled internally
        /// </summary>
        public uint Version
        {
            get => m_Version;
            set
            {
                // set new value
                m_Version = value;
                // notify event
                OnPropertyChanged(nameof(Version));
                // Set as string
                var strVer = (1000 + m_Version).ToString();
                ClientVersion = strVer.Substring(0, 1) + "." + strVer.Substring(1);
            }
        }
        /// <summary>
        /// Client version shown to the user
        /// </summary>
        public string ClientVersion
        {
            get => m_ClientVersion;
            private set
            {
                // set new value
                m_ClientVersion = value;
                // notify event
                OnPropertyChanged(nameof(ClientVersion));
            }
        }
        /// <summary>
        /// Check if the Pk2 has been correctly loaded
        /// </summary>
        public bool IsLoaded
        {
            get => m_IsLoaded;
            private set
            {
                // set new value
                m_IsLoaded = value;
                // notify event
                OnPropertyChanged(nameof(IsLoaded));
            }
        }
        /// <summary>
        /// Check if the launcher is on game config screen
        /// </summary>
        public bool IsViewingConfig
        {
            get => m_IsViewingConfig;
            set
            {
                // set new value
                m_IsViewingConfig = value;
                // notify event
                OnPropertyChanged(nameof(IsViewingConfig));
            }
        }
        /// <summary>
        /// The basic view config used by the game client
        /// </summary>
        public ConfigViewModel Config
        {
            get => m_Config;
            set
            {
                // set new value
                m_Config = value;
                // notify event
                OnPropertyChanged(nameof(Config));
            }
        }
        /// <summary>
        /// Check if the launcher is on game language config window
        /// </summary>
        public bool IsViewingLangConfig
        {
            get => m_IsViewingLangConfig;
            set
            {
                // set new value
                m_IsViewingLangConfig = value;
                // notify event
                OnPropertyChanged(nameof(IsViewingLangConfig));
            }
        }
        /// <summary>
        /// Language temporally selected as new option
        /// </summary>
        public int LangConfigIndex
        {
            get => m_LangConfigIndex;
            set
            {
                // set new value
                m_LangConfigIndex = value;
                // notify event
                OnPropertyChanged(nameof(LangConfigIndex));
            }
        }
        /// <summary>
        /// Check if the launcher is looking for update the client
        /// </summary>
        public bool IsCheckingUpdates
        {
            get => m_IsCheckingUpdates;
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
            get => m_IsUnderInspection;
            set
            {
                // set new value
                m_IsUnderInspection = value;
                // notify event
                OnPropertyChanged(nameof(IsUnderInspection));
            }
        }
        /// <summary>
        /// All notices loaded after checking updates
        /// </summary>
        public List<WebNoticeViewModel> WebNotices
        {
            get => m_WebNotices;
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
            get => m_SelectedWebNotice;
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
            get => m_IsUpdating;
            set
            {
                // set new value
                m_IsUpdating = value;
                // notify event
                OnPropertyChanged(nameof(IsUpdating));
            }
        }
        /// <summary>
        /// Get or sets the max. bytes quantity to be downloaded to apply patch
        /// </summary>
        public ulong UpdatingBytesMaxDownloading
        {
            get => m_UpdatingBytesMaxDownloading;
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
            get => m_UpdatingBytesDownloading;
            set
            {
                // set new value
                m_UpdatingBytesDownloading = value;
                // notify event
                OnPropertyChanged(nameof(UpdatingBytesDownloading));

                // Set percentage
                if (m_UpdatingBytesMaxDownloading != 0)
                    UpdatingPercentage = (int)(m_UpdatingBytesDownloading * 100ul / m_UpdatingBytesMaxDownloading);
            }
        }
        /// <summary>
        /// Get the current percentage from patch download
        /// </summary>
        public int UpdatingPercentage
        {
            get => m_UpdatingPercentage;
            set
            {
                if (m_UpdatingPercentage == value)
                    return;

                m_UpdatingPercentage = value;
                OnPropertyChanged(nameof(UpdatingPercentage));
            }
        }
        /// <summary>
        /// The current file being downloaded and imported
        /// </summary>
        public string UpdatingFilePath
        {
            get => m_UpdatingFilePath;
            set
            {
                m_UpdatingFilePath = value;
                OnPropertyChanged(nameof(UpdatingFilePath));
            }
        }
        /// <summary>
        /// Get the current file percentage being updated
        /// </summary>
        public int UpdatingFilePercentage
        {
            get => m_UpdatingFilePercentage;
            set
            {
                m_UpdatingFilePercentage = value;
                OnPropertyChanged(nameof(UpdatingFilePercentage));
            }
        }
        /// <summary>
        /// Check if the game can be started
        /// </summary>
        public bool CanStartGame
        {
            get => m_CanStartGame;
            set
            {
                // set new value
                m_CanStartGame = value;
                // notify event
                OnPropertyChanged(nameof(CanStartGame));
            }
        }
        /// <summary>
        /// Contains all assets to be displayed
        /// </summary>
        public LauncherAssets Assets { get; private set; }
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
        /// <summary>
        /// Set the language window viewing state
        /// </summary>
        public ICommand CommandToggleLangConfig { get; set; }
        /// <summary>
        /// Save the config file
        /// </summary>
        public ICommand CommandSaveConfig { get; set; }
        /// <summary>
        /// Save the config file
        /// </summary>
        public ICommand CommandSaveLangConfig { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        private LauncherViewModel()
        {
            #region Commands Setup
            // Windows commands
            CommandMinimize = new RelayCommand(() => {
                if (m_Window != null)
                    m_Window.WindowState = WindowState.Minimized;
            });
            CommandRestore = new RelayCommand(() => {
                if (m_Window != null)
                {
                    // Check the WindowState and change it
                    m_Window.WindowState = m_Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                }
            });
            CommandClose = new RelayCommand(Exit);
            CommandStartGame = new RelayCommand(()=> {
                // Starts the game but only if is ready and exists
                if (CanStartGame && File.Exists(LauncherSettings.CLIENT_EXECUTABLE)) { 
                    System.Diagnostics.Process.Start(LauncherSettings.CLIENT_EXECUTABLE, m_ClientArgs);
                    // Closing launcher
                    CommandClose.Execute(null);
                }
            });
            CommandOpenLink = new RelayParameterizedCommand((url) => {
                // Run link with default browser
                if (url is string link)
                    System.Diagnostics.Process.Start(link);
            });
            CommandToggleConfig = new RelayCommand(() => {
                IsViewingConfig = !IsViewingConfig;
            });
            CommandSaveConfig = new RelayCommand(() => {
                // Make sure pk2 it's not being used
                if (!IsUpdating)
                {
                    Config.Save();
                    IsViewingConfig = false;
                }
            });
            CommandToggleLangConfig = new RelayCommand(() => {
                // Update language selected currently
                if (!IsViewingLangConfig)
                    LangConfigIndex = Config.SupportedLanguageIndex;
                IsViewingLangConfig = !IsViewingLangConfig;
            });
            CommandSaveLangConfig = new RelayCommand(() => {
                // Make sure pk2 it's not being used
                if (!IsUpdating)
                {
                    // Avoid unchecked selection
                    if (LangConfigIndex != -1)
                    {
                        // Set new language selected
                        Config.SupportedLanguageIndex = LangConfigIndex;
                        Config.Save();
                    }
                    IsViewingLangConfig = false;
                }
            });
            #endregion

            // Init stuffs
            Initialize();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set the window this view model controls
        /// </summary>
        public void SetWindow(IWindow Window)
        {
            // Just save a reference
            m_Window = Window;
        }
        /// <summary>
        /// Show message to the user
        /// </summary>
        public void ShowMessage(string Text)
        {
            m_Window?.ShowMessage(Text,Title);
        }
        /// <summary>
        /// Check and loads the patch updates
        /// </summary>
        public void CheckUpdatesAsync()
        {
            if (!IsLoaded)
                return;

            IsCheckingUpdates = true;

            // Find the best connection
            long bestTime = 0;
            string hostAddress = null;

            var divIdx = 0;
            foreach (var div in m_DivisionInfo)
            {
                for (var hostIdx = 0; hostIdx < div.Value.Count; hostIdx++)
                {
                    var session = new Client();
                    // Connect to server and find the time used
                    if (session.Start(div.Value[hostIdx], m_Gateport, 5000, out var elapsedTime))
                    {
                        session.Stop();
                        // Check the best time
                        if (hostAddress == null || elapsedTime < bestTime)
                        {
                            hostAddress = div.Value[hostIdx];
                            elapsedTime = bestTime;
                            m_ClientArgs = "0 /" + m_Locale + " " + divIdx + " " + hostIdx;
                        }
                    }
                }
                divIdx++;
            }

            // Start gateway connection
            if (hostAddress != null)
            {
                m_GatewaySession = new Client();
                // Packet handlers
                m_GatewaySession.RegisterHandler(GatewayModule.Opcode.GLOBAL_IDENTIFICATION, GatewayModule.Server_GlobalIdentification);
                m_GatewaySession.RegisterHandler(GatewayModule.Opcode.SERVER_PATCH_RESPONSE, GatewayModule.Server_PatchResponse);
                m_GatewaySession.RegisterHandler(GatewayModule.Opcode.SERVER_SHARD_LIST_RESPONSE, GatewayModule.Server_ShardListResponse);
                m_GatewaySession.RegisterHandler(GatewayModule.Opcode.SERVER_WEB_NOTICE_RESPONSE, GatewayModule.Server_WebNoticeResponse);
                // Event handlers
                m_GatewaySession.OnConnect += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("Gateway: Session established");
                };
                m_GatewaySession.OnDisconnect += (s, e) => {
                    System.Diagnostics.Debug.WriteLine("Gateway: Session disconnected [" + e.Exception.Message + "]");
                };
                m_GatewaySession.Start(hostAddress, m_Gateport, 5000, out _);
            }
            else
            {
                // Not being able to connect to the server
                IsCheckingUpdates = false;
                IsUnderInspection = true;
                ShowMessage(LauncherSettings.MSG_INSPECTION);
            }
        }
        /// <summary>
        /// Exit from Launcher
        /// </summary>
        public void Exit()
        {
            App.Current.Dispatcher.Invoke(() => {
                m_GatewaySession?.Stop();
                m_Window?.Close();
            });
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Initialize all stuffs required for the connection and settings
        /// </summary>
        private void Initialize()
        {
            // Create mutex required to execute client
            CreateMutex(IntPtr.Zero, false, "Silkroad Online Launcher");
            CreateMutex(IntPtr.Zero, false, "Ready");
            // Load Pk2 Data
            Pk2Stream pk2Stream = null;
            try
            {
                // Load pk2 reader
                pk2Stream = new Pk2Stream(LauncherSettings.CLIENT_MEDIA_PK2_PATH, LauncherSettings.CLIENT_BLOWFISH_KEY);

                // Load assets from client
                Assets = new LauncherAssets(pk2Stream);

                // Extract essential stuffs for the process
                if (pk2Stream.TryGetDivisionInfo(out m_DivisionInfo) && pk2Stream.TryGetGateport(out m_Gateport))
                {
                    // Abort operations if host or port is not verified
                    if (!VerifyHosts(m_DivisionInfo) || !VerifyPort(m_Gateport))
                        return;

                    // Load settings
                    m_Config.Load(pk2Stream);
                    // continue extracting
                    if (pk2Stream.TryGetVersion(out m_Version)
                    && pk2Stream.TryGetLocale(out m_Locale))
                    {
                        IsLoaded = true;
                        // Force string to be updated
                        Version = m_Version;
                    }
                }
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory("Dump");
                File.WriteAllText("Dump/error.log", DateTime.Now.ToString() + ":" + ex);
                System.Diagnostics.Debug.WriteLine(ex);
                // Forced shutdown
                Application.Current.Shutdown();
            }
            finally
            {
                pk2Stream?.Dispose();
            }
        }
        /// <summary>
        /// Verify host used to start the game it's linked to launcher config
        /// </summary>
        private bool VerifyHosts(Dictionary<string, List<string>> divInfo)
        {
            // Verification not required
            if (LauncherSettings.CLIENT_VERIFY_HOST.Length == 0)
                return true;

            // Check every host from divisions
            foreach (var div in divInfo.Values)
            {
                foreach (var host in div)
                {
                    // Check if host is verified
                    foreach (var h in LauncherSettings.CLIENT_VERIFY_HOST)
                    {
                        if (h != host)
                            return false;
                    }
                }
            }

            // All host has been succeed
            return true;
        }
        /// <summary>
        /// Verify port used to start the game it's linked to launcher config
        /// </summary>
        private bool VerifyPort(int port)
        {
            return LauncherSettings.CLIENT_VERIFY_PORT == 0 || LauncherSettings.CLIENT_VERIFY_PORT == port;
        }
        #endregion
    }
}