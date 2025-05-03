using SilkroadLauncher.Utility;
using SRO.Common;
using SRO.Data;
using SRO.Data.Setting;
using SRO.PK2API;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SilkroadLauncher
{
    public class ConfigViewModel : BaseViewModel
    {
        #region WinAPI Methods
        /// <summary>
        /// Enum the list of availables video modes availables on the current system
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool EnumDisplaySettings(string DeviceName, int ModeNum, ref DEVMODE DevMode);
        [StructLayout(LayoutKind.Sequential)]
        private struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
        #endregion

        #region Private Members
        /// <summary>
        /// The basic config used to start the client
        /// </summary>
        private SilkCfg mSilkCfg = null;
        private readonly string PATH_SILKCFG = "SilkCfg.dat";
        /// <summary>
        /// Settings used by the client
        /// </summary>
        private SROptionSet mSROptionSet = null;
        private readonly string PATH_SROPTIONSET = "Setting\\SROptionSet.dat";
        /// <summary>
        /// The type file as raw of text
        /// </summary>
        private string m_TypeFile = null;
        /// <summary>
        /// Language found on type file
        /// </summary>
        private string m_Language = "Unknown";
        /// <summary>
        /// Language index currently selected
        /// </summary>
        private int m_SupportedLanguageIndex;
        #endregion

        #region Public Properties
        /// <summary>
        /// Config file version
        /// </summary>
        public uint Version => mSilkCfg.Version;
        /// <summary>
        /// Game resolutions supported
        /// </summary>
        public List<WindowResolution> SupportedResolutions { get; }
        /// <summary>
        /// Game resolution
        /// </summary>
        public WindowResolution Resolution
        {
            get => mSilkCfg.Resolution;
            set
            {
                mSilkCfg.Resolution = value;
                OnPropertyChanged(nameof(Resolution));
            }
        }
        /// <summary>
        /// Game resolutions supported
        /// </summary>
        public List<SilkCfg.Brightness> SupportedBrightness { get; }

        /// <summary>
        /// Game brightness
        /// </summary>
        public SilkCfg.Brightness Brightness
        {
            get => mSilkCfg.BrightnessType;
            set
            {
                mSilkCfg.BrightnessType = value;
                OnPropertyChanged(nameof(Brightness));
            }
        }
        /// <summary>
        /// Game graphics supported
        /// </summary>
        public List<SilkCfg.Graphic> SupportedGraphics { get; }
        /// <summary>
        /// Game graphics
        /// </summary>
        public SilkCfg.Graphic Graphics
        {
            get => mSilkCfg.GraphicType1;
            set
            {
                mSilkCfg.GraphicType1 = value;
                OnPropertyChanged(nameof(Graphics));
            }
        }
        /// <summary>
        /// Using sound in game
        /// </summary>
        public bool IsSoundEnabled
        {
            get => mSilkCfg.IsSoundEnabled;
            set
            {
                mSilkCfg.IsSoundEnabled = value;
                OnPropertyChanged(nameof(IsSoundEnabled));
            }
        }
        /// <summary>
        /// Using window mode
        /// </summary>
        public bool IsWindowMode
        {
            get => (bool)mSROptionSet.Get(SROptionSet.ID.Video_WindowMode_isWindowMode);
            set
            {
                mSROptionSet.Set(SROptionSet.ID.Video_WindowMode_isWindowMode, value);
                OnPropertyChanged(nameof(IsWindowMode));
            }
        }
        /// <summary>
        /// All languages supported by the game
        /// </summary>
        public List<string> SupportedLanguages { get; }
        /// <summary>
        /// Language index selected by user
        /// </summary>
        public int SupportedLanguageIndex
        {
            get => m_SupportedLanguageIndex;
            set
            {
                m_SupportedLanguageIndex = value;
                OnPropertyChanged(nameof(SupportedLanguageIndex));
                // Set the language internally
                m_Language = LauncherSettings.CLIENT_LANGUAGE_SUPPORTED[value];
                // Raise event
                OnPropertyChanged(nameof(Language));
            }
        }
        /// <summary>
        /// Language mask selected by user
        /// </summary>
        public string Language => LauncherSettings.CLIENT_LANGUAGE_SUPPORTED_MASK[SupportedLanguageIndex];
        #endregion

        #region Constructor
        public ConfigViewModel()
        {
            // Load available resolutions to display
            SupportedResolutions = new List<WindowResolution>();
            var mode = new DEVMODE();
            var minPixels = 800 * 600;
            for (int i = 0; EnumDisplaySettings(null, i, ref mode); i++)
            {
                // Avoid lowest resolutions
                if (mode.dmPelsWidth * mode.dmPelsHeight < minPixels)
                    continue;
                // Add only one per resolution
                var resolution = new WindowResolution((uint)mode.dmPelsWidth, (uint)mode.dmPelsHeight);
                if (!SupportedResolutions.Contains(resolution))
                    SupportedResolutions.Add(resolution);
            }

            // Default brightness
            SupportedBrightness = new List<SilkCfg.Brightness>(){
                SilkCfg.Brightness.VeryDark,
                SilkCfg.Brightness.Dark,
                SilkCfg.Brightness.Normal,
                SilkCfg.Brightness.Bright,
                SilkCfg.Brightness.VeryBright
            };
            // Default graphics
            SupportedGraphics = new List<SilkCfg.Graphic>()
            {
                SilkCfg.Graphic.Low,
                SilkCfg.Graphic.Middle,
                SilkCfg.Graphic.High
            };
            // Set default configs & synchronize
            mSilkCfg = new SilkCfg()
            {
                Resolution = SupportedResolutions.First()
            };
            mSROptionSet = new SROptionSet();

            mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_ResolutionWidth, mSilkCfg.Resolution.Width);
            mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_ResolutionHeight, mSilkCfg.Resolution.Height);
            mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_Brightness, (byte)mSilkCfg.BrightnessType);

            // Set languages supported
            SupportedLanguages = new List<string>(LauncherSettings.CLIENT_LANGUAGE_SUPPORTED_MASK);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load settings or create a new one
        /// </summary>
        public void Load(Pk2Stream pk2Reader)
        {
            // Loads language from pk2
            TryLoadTypeFile(pk2Reader);

            // If SROptionSet exists, try to update SilkCfg with those settings
            if (TryLoadSROptionSet())
            {
                if (!ValidateSROptionSet())
                    SaveSROptionSet();
                // Try to load SilkCfg and fix it
                if (TryLoadSilkCfg())
                    ValidateSilkCfg();
                SyncSilkCfg();
                SaveSilkCfg();
            }
            // Otherwise keep the original behavior, first Load SilkCfg, then SROptionSet
            else
            {
                if (!TryLoadSilkCfg() || !ValidateSilkCfg())
                    SaveSilkCfg();
                // Create SROptionSet and save it
                ValidateSROptionSet();
                SyncSROptionSet();
                SaveSROptionSet();
            }
        }
        /// <summary>
        /// Save the current settings
        /// </summary>
        public void Save()
        {
            SaveTypeFile();
            SaveSilkCfg();
            SyncSROptionSet();
            SaveSROptionSet();
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Try to load the SilkCfg file
        /// </summary>
        private bool TryLoadSilkCfg()
        {
            if (File.Exists(PATH_SILKCFG))
            {
                try
                {
                    var silkcfg = new SilkCfg();
                    silkcfg.Load(new FileStream(PATH_SILKCFG, FileMode.Open, FileAccess.Read));
                    mSilkCfg = silkcfg;
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            return false;
        }
        /// <summary>
        /// Make sure <see cref="mSilkCfg"/> is fixed for the current user. Returns true if everything seems fine.
        /// </summary>
        private bool ValidateSilkCfg()
        {
            var result = true;

            // Make adjustments for the current user if necessary
            if (!SupportedResolutions.Contains(mSilkCfg.Resolution))
            {
                mSilkCfg.Resolution = SupportedResolutions.First();
                result = false;
            }
            if (!SupportedGraphics.Contains(mSilkCfg.GraphicType1))
            {
                mSilkCfg.GraphicType1 = SilkCfg.Graphic.Middle;
                result = false;
            }
            if (!SupportedBrightness.Contains(mSilkCfg.BrightnessType))
            {
                mSilkCfg.BrightnessType = SilkCfg.Brightness.Normal;
                result = false;
            }
            if (mSilkCfg.GraphicTypeSelected != 1)
            {
                mSilkCfg.GraphicTypeSelected = 1;
                result = false;
            }

            return result;
        }
        /// <summary>
        /// Save the SilkCfg setting
        /// </summary>
        private void SaveSilkCfg()
        {
            try
            {
                // Create path
                var dir = Path.GetDirectoryName(PATH_SILKCFG);
                if (dir != string.Empty)
                    Directory.CreateDirectory(dir);

                mSilkCfg.Save(new FileStream(PATH_SILKCFG, FileMode.Create, FileAccess.Write));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Try to load the SROptionSet file
        /// </summary>
        private bool TryLoadSROptionSet()
        {
            if (File.Exists(PATH_SROPTIONSET))
            {
                try
                {
                    var sroptionset = new SROptionSet();
                    sroptionset.Load(new FileStream(PATH_SROPTIONSET, FileMode.Open, FileAccess.Read));
                    mSROptionSet = sroptionset;
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            return false;
        }
        /// <summary>
        /// Make sure <see cref="mSROptionSet"/> is fixed for the current user. Returns true if everything seems fine.
        /// </summary>
        private bool ValidateSROptionSet()
        {
            var result = true;

            // Make adjustments for the current user if necessary
            var graphic = (SilkCfg.Graphic)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_Type);
            if (graphic != SilkCfg.Graphic.Unchanged && !SupportedGraphics.Contains(graphic))
            {
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_Type, (byte)SilkCfg.Graphic.Middle);
                result = false;
            }
            var brightness = (SilkCfg.Brightness)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_Brightness);
            if (!SupportedBrightness.Contains(brightness))
            {
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_Brightness, (byte)SilkCfg.Brightness.Normal);
                result = false;
            }
            var wr = new WindowResolution((uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_ResolutionWidth), (uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_ResolutionHeight));
            if (!SupportedResolutions.Contains(wr))
            {
                var r = SupportedResolutions.First();
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_ResolutionWidth, r.Width);
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_ResolutionHeight, r.Height);
                result = false;
            }
            wr = new WindowResolution((uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic2_ResolutionWidth), (uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic2_ResolutionHeight));
            if (!SupportedResolutions.Contains(wr))
            {
                var r = SupportedResolutions.Last();
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic2_ResolutionWidth, r.Width);
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic2_ResolutionHeight, r.Height);
                result = false;
            }

            return result;
        }
        /// <summary>
        /// Save the SROptionSet setting
        /// </summary>
        private void SaveSROptionSet()
        {
            try
            {
                // Create path
                var dir = Path.GetDirectoryName(PATH_SROPTIONSET);
                if (dir != string.Empty)
                    Directory.CreateDirectory(dir);

                mSROptionSet.Save(new FileStream(PATH_SROPTIONSET, FileMode.Create, FileAccess.Write));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// Make launcher configs (SilkCfg) reflect game settings (SROptionSet). Returns true if something has been synchronized.
        /// </summary>
        private bool SyncSilkCfg()
        {
            var result = false;

            var wr = new WindowResolution((uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_ResolutionWidth), (uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_ResolutionHeight));
            if(mSilkCfg.Resolution != wr)
            {
                mSilkCfg.Resolution = wr;
                result = true;
            }
            var soundEnabled = (bool)mSROptionSet.Get(SROptionSet.ID.Audio_BackgroundVolume_BackgroundVolumeCheckbox) || (bool)mSROptionSet.Get(SROptionSet.ID.Audio_EnvironmentalVolume_EnvironmentalVolumeCheckbox) || (bool)mSROptionSet.Get(SROptionSet.ID.Audio_FXVolume_FXVolumeCheckBox);
            if(mSilkCfg.IsSoundEnabled != soundEnabled)
            {
                mSilkCfg.IsSoundEnabled = soundEnabled;
                result = true;
            }
            var brightness = (SilkCfg.Brightness)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_Brightness);
            if (mSilkCfg.BrightnessType != brightness)
            {
                mSilkCfg.BrightnessType = brightness;
                result = true;
            }

            return result;
        }
        /// <summary>
        /// Make game settings (SROptionSet) reflect launcher settings (SilkCfg). Returns true if something has been synchronized.
        /// </summary>
        private bool SyncSROptionSet()
        {
            var result = false;

            var wr = new WindowResolution((uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_ResolutionWidth), (uint)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_ResolutionHeight));
            if (mSilkCfg.Resolution != wr)
            {
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_ResolutionWidth, mSilkCfg.Resolution.Width);
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_ResolutionHeight, mSilkCfg.Resolution.Height);
                result = true;
            }
            var soundEnabled = (bool)mSROptionSet.Get(SROptionSet.ID.Audio_BackgroundVolume_BackgroundVolumeCheckbox) || (bool)mSROptionSet.Get(SROptionSet.ID.Audio_EnvironmentalVolume_EnvironmentalVolumeCheckbox) || (bool)mSROptionSet.Get(SROptionSet.ID.Audio_FXVolume_FXVolumeCheckBox);
            if (mSilkCfg.IsSoundEnabled != soundEnabled)
            {
                mSROptionSet.Set(SROptionSet.ID.Audio_BackgroundVolume_BackgroundVolumeCheckbox, mSilkCfg.IsSoundEnabled);
                mSROptionSet.Set(SROptionSet.ID.Audio_EnvironmentalVolume_EnvironmentalVolumeCheckbox, mSilkCfg.IsSoundEnabled);
                mSROptionSet.Set(SROptionSet.ID.Audio_FXVolume_FXVolumeCheckBox, mSilkCfg.IsSoundEnabled);
                result = true;
            }
            var brightness = (SilkCfg.Brightness)mSROptionSet.Get(SROptionSet.ID.Video_Graphic1_Brightness);
            if (mSilkCfg.BrightnessType != brightness)
            {
                mSROptionSet.Set(SROptionSet.ID.Video_Graphic1_Brightness, (byte)mSilkCfg.BrightnessType);
                result = true;
            }

            return result;
        }
        /// <summary>
        /// Try to load the Type file from the Pk2
        /// </summary>
        /// <param name="Pk2Stream">Pk2 used to search</param>
        /// <returns>Return success</returns>
        private bool TryLoadTypeFile(Pk2Stream Pk2Stream)
        {
            var temp = Pk2Stream.GetFileText("Type.txt");
            // Check if file has been found
            if (temp != null)
            {
                // Analyze the file and extract language
                var match = Regex.Match(temp, "Language[ ]{0,1}=[ ]{0,1}[\"]{0,1}([a-zA-Z]*)[\"]{0,1}");
                if (match.Success)
                {
                    m_TypeFile = temp;
                    // Try to find the index selected
                    for (int i = 0; i < LauncherSettings.CLIENT_LANGUAGE_SUPPORTED.Length; i++)
                    {
                        if (LauncherSettings.CLIENT_LANGUAGE_SUPPORTED[i] == match.Groups[1].Value)
                        {
                            SupportedLanguageIndex = i;
                            return true;
                        }
                    }
                    SupportedLanguageIndex = 0;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Save the type file but only if has been loaded before
        /// </summary>
        private void SaveTypeFile()
        {
            if (m_TypeFile != null)
            {
                // Replace value
                m_TypeFile = Regex.Replace(m_TypeFile, "Language[ ]{0,1}=[ ]{0,1}[\"]{0,1}([a-zA-Z]*)[\"]{0,1}", "Language = \"" + m_Language + "\"");
                // Import to Pk2
                try
                {
                    using (var pk2 = new Pk2Stream(LauncherSettings.CLIENT_MEDIA_PK2_PATH, LauncherSettings.CLIENT_BLOWFISH_KEY))
                        pk2.AddFile("Type.txt", Encoding.UTF8.GetBytes(m_TypeFile));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }
        #endregion
    }
}
