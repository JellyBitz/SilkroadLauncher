using System.IO;
using SilkroadLauncher.SilkroadCommon;
using System.Collections.Generic;
using System;
using SilkroadLauncher.SilkroadCommon.Setting;
using System.Text.RegularExpressions;
using Pk2ReaderAPI;
using Pk2WriterAPI;

namespace SilkroadLauncher
{
    public class ConfigViewModel : BaseViewModel
    {
        #region Private Members
        /// <summary>
        /// SilkCfg file location
        /// </summary>
        string m_SilkCfgPath = "SilkCfg.dat";
        /// <summary>
        /// The basic config used to start the client
        /// </summary>
        private SilkCfg m_SilkCfg;
        /// <summary>
        /// SilkCfg file location
        /// </summary>
        string m_SROptionSetPath = "Setting\\SROptionSet.dat";
        /// <summary>
        /// The basic settings used by the client
        /// </summary>
        private SROptionSet m_SROptionSet;
        /// <summary>
        /// The type file as raw of text
        /// </summary>
        private string m_TypeFile = null;
        /// <summary>
        /// Language found on type file
        /// </summary>
        private string m_Language = "Unknown";
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
        /// In game language
        /// </summary>
        public string Language
        {
            get { return m_Language; }
            set
            {
                m_Language = value;
                OnPropertyChanged(nameof(Language));
            }
        }
        #endregion

        #region Constructor
        public ConfigViewModel()
        {
            // Set default SilkCfg.dat
            m_SilkCfg = new SilkCfg();
            SupportedResolutions = new List<SilkCfg.WindowResolution>(){
                new SilkCfg.WindowResolution(800,600),
                new SilkCfg.WindowResolution(1024,768),
                new SilkCfg.WindowResolution(1280,720),
                new SilkCfg.WindowResolution(1280,800),
                new SilkCfg.WindowResolution(1280,1024),
                new SilkCfg.WindowResolution(1366,768),
                new SilkCfg.WindowResolution(1440,900),
                new SilkCfg.WindowResolution(1600,900),
                new SilkCfg.WindowResolution(1680,1050),
                new SilkCfg.WindowResolution(1920,1080)
            };
            SupportedBrightness = new List<SilkCfg.Brightness>(){
                SilkCfg.Brightness.VeryDark,
                SilkCfg.Brightness.Dark,
                SilkCfg.Brightness.Normal,
                SilkCfg.Brightness.Bright,
                SilkCfg.Brightness.VeryBright
            };
            SupportedGraphics = new List<SilkCfg.Graphic>()
            {
                SilkCfg.Graphic.Low,
                SilkCfg.Graphic.Middle,
                SilkCfg.Graphic.High
            };
            // Set default SROptionSet.dat
            m_SROptionSet = new SROptionSet();
            // Set languages supported
            SupportedLanguages = new List<string>()
            {
                "English",
                "Vietnam"
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Try to load the SilkCfg file
        /// </summary>
        public bool LoadSilkCfg()
        {
            if (File.Exists(m_SilkCfgPath))
            {
                BinaryReader reader = null;
                try
                {
                    reader = new BinaryReader(new FileStream(m_SilkCfgPath, FileMode.Open, FileAccess.Read));
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
                    }
                } catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return false;
                } finally {
                    reader?.Close();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Reset the SilkCfg setting
        /// </summary>
        public void ResetSilkCfg()
        {
            m_SilkCfg = new SilkCfg();
        }
        /// <summary>
        /// Save the SilkCfg setting
        /// </summary>
        public void SaveSilkCfg()
        {
            BinaryWriter writer = null;
            try
            {
                // Create location
                string dir = Path.GetDirectoryName(m_SilkCfgPath);
                if (dir != string.Empty && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                writer = new BinaryWriter(new FileStream(m_SilkCfgPath, FileMode.Create, FileAccess.Write));
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
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            } finally {
                writer?.Close();
            }
        }
        /// <summary>
        /// Try to load the SROptionSet file
        /// </summary>
        public bool LoadSROptionSet()
        {
            if (File.Exists(m_SROptionSetPath))
            {
                BinaryReader reader = null;
                try
                {
                    reader = new BinaryReader(new FileStream(m_SROptionSetPath, FileMode.Open, FileAccess.Read));
                    // Read config structure by version
                    m_SROptionSet.Version = reader.ReadUInt32();
                    m_SROptionSet.unkByte01 = reader.ReadByte();
                    m_SROptionSet.unkUInt01 = reader.ReadUInt32();

                    while(reader.BaseStream.Position < reader.BaseStream.Length){
                        SROptionSet.OptionID id = (SROptionSet.OptionID)reader.ReadUInt32();
                        if(m_SROptionSet.Options.TryGetValue(id,out object value)){
                            if (value is bool) value = reader.ReadBoolean();
                            else if (value is byte) value = reader.ReadByte();
                            else if (value is ushort) value = reader.ReadUInt16();
                            else if (value is uint) value = reader.ReadUInt32();
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"SROptionSet: ID [{id}] not found");
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return false;
                }
                finally
                {
                    reader?.Close();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Reset the SROptionSet setting
        /// </summary>
        public void ResetSROptionSet()
        {
            m_SROptionSet = new SROptionSet();
        }
        /// <summary>
        /// Save the SROptionSet setting
        /// </summary>
        public void SaveSROptionSet()
        {
            BinaryWriter writer = null;
            try
            {
                // Create location
                string dir = Path.GetDirectoryName(m_SROptionSetPath);
                if (dir != string.Empty && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                writer = new BinaryWriter(new FileStream(m_SROptionSetPath, FileMode.Create, FileAccess.Write));
                // Write config structure by version
                writer.Write(m_SROptionSet.Version);
                writer.Write(m_SROptionSet.unkByte01);
                writer.Write(m_SROptionSet.unkUInt01);
                foreach(var k_v in m_SROptionSet.Options)
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
        public bool LoadTypeFile(Pk2Reader Pk2Reader)
        {
            var temp = Pk2Reader.GetFileText("Type.txt");
            // Check if file has been found
            if (temp != null)
            {
                // Analyze the file and extract language
                var match = Regex.Match(temp, "Language[ ]{0,1}=[ ]{0,1}[\"]{0,1}([a-zA-Z]*)[\"]{0,1}");
                if (match.Success)
                {
                    m_TypeFile = temp;
                    m_Language = match.Groups[1].Value;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Save the type file but only if has been loaded before
        /// </summary>
        public void SaveTypeFile()
        {
            if (m_TypeFile != null)
            {
                // Replace value
                m_TypeFile = Regex.Replace(m_TypeFile,"Language[ ]{0,1}=[ ]{0,1}[\"]{0,1}([a-zA-Z]*)[\"]{0,1}", "Language = \""+ Language + "\"");
                // Import to Pk2
                if (Pk2Writer.Initialize("GFXFileManager.dll"))
                {
                    if(Pk2Writer.Open("Media.pk2", LauncherViewModel.Instance.BlowfishKey))
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
        /// <summary>
        /// Save the current settings
        /// </summary>
        public void Save()
        {
            SaveSilkCfg();
            SaveSROptionSet();
            SaveTypeFile();
        }
        #endregion
    }
}
