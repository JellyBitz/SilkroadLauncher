using System.IO;
using SilkroadLauncher.SilkroadCommon;
using System.Collections.Generic;
using System;
using SilkroadLauncher.SilkroadCommon.Setting;
using System.Text.RegularExpressions;
using Pk2ReaderAPI;
using Pk2WriterAPI;
using System.Runtime.InteropServices;
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
        private SilkCfg m_SilkCfg;
        private readonly string PATH_SILKCFG = "SilkCfg.dat";
        /// <summary>
        /// Settings used by the client
        /// </summary>
        private SROptionSet m_SROptionSet;
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
        public uint Version { get { return m_SilkCfg.Version; } }
        /// <summary>
        /// Game resolutions supported
        /// </summary>
        public List<SilkCfg.WindowResolution> SupportedResolutions { get; }
        /// <summary>
        /// Game resolution
        /// </summary>
        public SilkCfg.WindowResolution Resolution
        {
            get { return m_SilkCfg.Resolution; }
            set
            {
                m_SilkCfg.Resolution = value;
                m_SROptionSet.Options[SROptionSet.OptionID.Graphic01_Width] = value.Width;
                m_SROptionSet.Options[SROptionSet.OptionID.Graphic01_Height] = value.Height;
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
            get { return m_SilkCfg.BrightnessType; }
            set
            {
                m_SilkCfg.BrightnessType = value;
                m_SROptionSet.Options[SROptionSet.OptionID.Graphic01_Brightness] = (byte)value;
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
            get { return m_SilkCfg.GraphicType; }
            set
            {
                m_SilkCfg.GraphicType = value;
                OnPropertyChanged(nameof(Graphics));
            }
        }
        /// <summary>
        /// Using sound in game
        /// </summary>
        public bool IsSoundEnabled
        {
            get { return m_SilkCfg.IsSoundEnabled; }
            set
            {
                m_SilkCfg.IsSoundEnabled = value;
                OnPropertyChanged(nameof(IsSoundEnabled));
            }
        }
        /// <summary>
        /// Using window mode
        /// </summary>
        public bool IsWindowMode
        {
            get { return (bool)m_SROptionSet.Options[SROptionSet.OptionID.IsWindowMode]; }
            set
            {
                m_SROptionSet.Options[SROptionSet.OptionID.IsWindowMode] = value;
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
            get { return m_SupportedLanguageIndex; }
            set
            {
                m_SupportedLanguageIndex = value;
                OnPropertyChanged(nameof(SupportedLanguageIndex));
                // Set the language internally
                m_Language = LauncherSettings.CLIENT_LANGUAGE_SUPPORTED[value];
            }
        }
        #endregion

        #region Constructor
        public ConfigViewModel()
        {
            // Set default SilkCfg.dat
            m_SilkCfg = new SilkCfg();

            // Load available resolutions to display
            SupportedResolutions = new List<SilkCfg.WindowResolution>();
            DEVMODE mode = new DEVMODE();
            var minPixels = 800 * 600;
            for (int i = 0; EnumDisplaySettings(null, i, ref mode); i++)
            {
                // Avoid lowest resolutions
                if (mode.dmPelsWidth * mode.dmPelsHeight < minPixels)
                    continue;
                // Add only one per resolution
                var resolution = new SilkCfg.WindowResolution((uint)mode.dmPelsWidth, (uint)mode.dmPelsHeight);
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
            // Set default SROptionSet.dat
            m_SROptionSet = new SROptionSet();
            // Set languages supported
            SupportedLanguages = new List<string>(LauncherSettings.CLIENT_LANGUAGE_SUPPORTED_MASK);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load settings or create a new one
        /// </summary>
        public void Load(Pk2Reader pk2Reader)
        {
            // Loads language from pk2
            LoadTypeFile(pk2Reader);

            // Try to load configs or create a new one
            bool createNew = false;
            if (!LoadSilkCfg())
            {
                m_SilkCfg = new SilkCfg();
                createNew = true;
            }
            if (!LoadSROptionSet())
            {
                m_SROptionSet = new SROptionSet();
                createNew = true;
            }
            BindConfigs();

            // Save changes created
            if (createNew)
                Save();
        }
        /// <summary>
        /// Save the current settings
        /// </summary>
        public void Save()
        {
            SaveTypeFile();
            SaveSilkCfg();
            SaveSROptionSet();
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Set launcher configs to reflect game settings
        /// </summary>
        private void BindConfigs()
        {
            m_SilkCfg.Resolution = new SilkCfg.WindowResolution((uint)m_SROptionSet.Options[SROptionSet.OptionID.Graphic01_Width], (uint)m_SROptionSet.Options[SROptionSet.OptionID.Graphic01_Height]);
            m_SilkCfg.BrightnessType = (SilkCfg.Brightness)m_SROptionSet.Options[SROptionSet.OptionID.Graphic01_Brightness];
        }
        /// <summary>
        /// Try to load the SilkCfg file
        /// </summary>
        private bool LoadSilkCfg()
        {
            if (File.Exists(PATH_SILKCFG))
            {
                BinaryReader reader = null;
                try
                {
                    reader = new BinaryReader(new FileStream(PATH_SILKCFG, FileMode.Open, FileAccess.Read));
                    // Read config structure by version
                    m_SilkCfg.Version = reader.ReadUInt32();
                    if (m_SilkCfg.Version < 4)
                    {
                        m_SilkCfg.unkUint01 = reader.ReadUInt32();
                        // Read game resolution and ignore if is not supported
                        SilkCfg.WindowResolution w = new SilkCfg.WindowResolution(reader.ReadUInt32(), reader.ReadUInt32());
                        if (SupportedResolutions.Contains(w))
                            m_SilkCfg.Resolution = w;
                        // Graphics #1
                        SilkCfg.Graphic g = (SilkCfg.Graphic)reader.ReadByte();
                        if (SupportedGraphics.Contains(g))
                            m_SilkCfg.GraphicType = g;
                        m_SilkCfg.unkByte01 = reader.ReadByte();
                        // Sound 
                        m_SilkCfg.IsSoundEnabled = reader.ReadBoolean();
                        m_SilkCfg.unkByte02 = reader.ReadByte();
                        if (m_SilkCfg.Version == 3)
                        {
                            // Read graphics #2
                            SilkCfg.Graphic g2 = (SilkCfg.Graphic)reader.ReadByte();
                            // Graphics choosen
                            byte gIndex = reader.ReadByte();
                            // Just handling one graphic from GUI
                            if (gIndex == 2 && SupportedGraphics.Contains(g2))
                                m_SilkCfg.GraphicType = g2;
                        }
                        m_SilkCfg.unkByte03 = reader.ReadByte();
                    }
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                finally
                {
                    reader?.Close();
                }
            }
            return false;
        }
        /// <summary>
        /// Save the SilkCfg setting
        /// </summary>
        private void SaveSilkCfg()
        {
            BinaryWriter writer = null;
            try
            {
                // Create location
                string dir = Path.GetDirectoryName(PATH_SILKCFG);
                if (dir != string.Empty && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                writer = new BinaryWriter(new FileStream(PATH_SILKCFG, FileMode.Create, FileAccess.Write));
                // Write config structure by version
                writer.Write(m_SilkCfg.Version);
                if (m_SilkCfg.Version < 4)
                {
                    writer.Write(m_SilkCfg.unkUint01);
                    // Game resolution
                    writer.Write(m_SilkCfg.Resolution.Width);
                    writer.Write(m_SilkCfg.Resolution.Height);
                    // Graphics #1
                    writer.Write((byte)m_SilkCfg.GraphicType);
                    writer.Write(m_SilkCfg.unkByte01);
                    // Sound
                    writer.Write(m_SilkCfg.IsSoundEnabled);
                    writer.Write(m_SilkCfg.unkByte02);
                    if (m_SilkCfg.Version == 3)
                    {
                        // Graphics #2
                        writer.Write((byte)m_SilkCfg.GraphicType);
                        // Graphics choosen always set #1
                        writer.Write((byte)1);
                    }
                    writer.Write(m_SilkCfg.unkByte03);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                writer?.Close();
            }
        }
        /// <summary>
        /// Try to load the SROptionSet file
        /// </summary>
        private bool LoadSROptionSet()
        {
            if (File.Exists(PATH_SROPTIONSET))
            {
                BinaryReader reader = null;
                try
                {
                    reader = new BinaryReader(new FileStream(PATH_SROPTIONSET, FileMode.Open, FileAccess.Read));
                    // Read config structure by version
                    m_SROptionSet.Version = reader.ReadUInt32();
                    m_SROptionSet.unkByte01 = reader.ReadByte();
                    m_SROptionSet.unkUInt01 = reader.ReadUInt32();

                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        SROptionSet.OptionID id = (SROptionSet.OptionID)reader.ReadUInt32();
                        if (m_SROptionSet.Options.TryGetValue(id, out object value))
                        {
                            if (value is bool) value = reader.ReadBoolean();
                            else if (value is byte) value = reader.ReadByte();
                            else if (value is ushort) value = reader.ReadUInt16();
                            else if (value is uint) value = reader.ReadUInt32();
                            // Update the saved value
                            m_SROptionSet.Options[id] = value;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"SROptionSet: ID [{id}] not found, loading aborted!");
                            break;
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                finally
                {
                    reader?.Close();
                }
            }
            return false;
        }
        /// <summary>
        /// Save the SROptionSet setting
        /// </summary>
        private void SaveSROptionSet()
        {
            BinaryWriter writer = null;
            try
            {
                // Create location
                string dir = Path.GetDirectoryName(PATH_SROPTIONSET);
                if (dir != string.Empty && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                writer = new BinaryWriter(new FileStream(PATH_SROPTIONSET, FileMode.Create, FileAccess.Write));
                // Write config structure by version
                writer.Write(m_SROptionSet.Version);
                writer.Write(m_SROptionSet.unkByte01);
                writer.Write(m_SROptionSet.unkUInt01);
                foreach (var k_v in m_SROptionSet.Options)
                {
                    // ID
                    writer.Write((uint)k_v.Key);
                    // Value
                    if (k_v.Value is bool _bool) writer.Write(_bool);
                    else if (k_v.Value is byte _byte) writer.Write(_byte);
                    else if (k_v.Value is ushort _ushort) writer.Write(_ushort);
                    else if (k_v.Value is uint _uint) writer.Write(_uint);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                writer?.Close();
            }
        }
        /// <summary>
        /// Try to load the Type file from the Pk2
        /// </summary>
        /// <param name="Pk2Reader">Pk2 used to search</param>
        /// <returns>Return success</returns>
        private bool LoadTypeFile(Pk2Reader Pk2Reader)
        {
            var temp = Pk2Reader.GetFileText("Type.txt");
            // Check if file has been found
            if (temp != null)
            {
                // Analyze the file and extract language
                var match = Regex.Match(temp, "Language[ ]{0,1}=[ ]{0,1}[\"]{0,1}([a-zA-Z]*)[\"]{0,1}");
                if (match.Success)
                {
                    // Try to find the index selected
                    for (int i = 0; i < LauncherSettings.CLIENT_LANGUAGE_SUPPORTED.Length; i++)
                    {
                        if (LauncherSettings.CLIENT_LANGUAGE_SUPPORTED[i] == match.Groups[1].Value)
                        {
                            m_TypeFile = temp;
                            SupportedLanguageIndex = i;
                            return true;
                        }
                    }
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
                if (Pk2Writer.Initialize("GFXFileManager.dll"))
                {
                    if (Pk2Writer.Open(LauncherSettings.PATH_PK2_MEDIA, LauncherSettings.CLIENT_BLOWFISH_KEY))
                    {
                        // Create a temporary file
                        if (!Directory.Exists("Temp"))
                            Directory.CreateDirectory("Temp");
                        File.WriteAllText("Temp\\Type.txt", m_TypeFile);
                        // Edit Pk2
                        Pk2Writer.ImportFile("Type.txt", "Temp\\Type.txt");
                        // Delete temporary file
                        File.Delete("Temp\\Type.txt");
                        // Close Pk2
                        Pk2Writer.Close();
                        Pk2Writer.Deinitialize();
                    }
                }
            }
        }
        #endregion
    }
}
