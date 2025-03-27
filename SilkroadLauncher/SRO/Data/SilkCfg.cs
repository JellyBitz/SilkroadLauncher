using SRO.Common;
using System.IO;

namespace SRO.Data
{
    /// <summary>
    /// SilkCfg.dat file format
    /// <para>
    /// <see cref="https://github.com/DummkopfOfHachtenduden/SilkroadDoc/wiki/SilkCfg"/>
    /// </para>
    /// </summary>
    public class SilkCfg
    {
        #region Public Properties
        public uint Version { get; set; } = 3;
        public uint unkUint01 { get; set; } = 1;
        public WindowResolution Resolution { get; set; } = new WindowResolution(800, 600);
        public Graphic GraphicType1 { get; set; } = Graphic.Middle;
        public Brightness BrightnessType { get; set; } = Brightness.Normal;
        public byte unkByte01 { get; set; } = 1;
        public bool IsSoundEnabled { get; set; } = true;
        public byte LanguageID { get; set; } = 0;
        public Graphic GraphicType2 { get; set; } = Graphic.High;
        public byte GraphicTypeSelected { get; set; } = 1;
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads a SilkCfg.dat file format
        /// </summary>
        public void Load(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                Version = br.ReadUInt32();
                unkUint01 = br.ReadUInt32();
                Resolution = new WindowResolution(br.ReadUInt32(), br.ReadUInt32());
                GraphicType1 = (Graphic)br.ReadByte();
                BrightnessType = (Brightness)br.ReadByte();
                unkByte01 = br.ReadByte();
                IsSoundEnabled = br.ReadBoolean();
                LanguageID = br.ReadByte();
                // Read 2nd Graphics Slot
                if (Version == 3)
                {
                    GraphicType2 = (Graphic)br.ReadByte();
                    GraphicTypeSelected = br.ReadByte();
                }
            }
        }
        /// <summary>
        /// Saves a SilkCfg.dat file format
        /// </summary>
        public void Save(Stream stream)
        {
            using (var bw = new BinaryWriter(stream))
            {
                bw.Write(Version);
                bw.Write(unkUint01);
                bw.Write(Resolution.Width);
                bw.Write(Resolution.Height);
                bw.Write((byte)GraphicType1);
                bw.Write((byte)BrightnessType);
                bw.Write(unkByte01);
                bw.Write(IsSoundEnabled);
                bw.Write(LanguageID);
                if(Version == 3)
                {
                    bw.Write((byte)GraphicType2);
                    bw.Write(GraphicTypeSelected);
                }
            }
        }
        #endregion

        #region Class & Enums Helpers
        /// <summary>
        /// Graphic visual type
        /// </summary>
        public enum Graphic : byte
        {
            Low = 0,
            Middle = 1,
            High = 2,
            Large = 3,
            Unchanged = 4
        }
        /// <summary>
        /// Screen brightness
        /// </summary>
        public enum Brightness : byte
        {
            VeryDark = 0,
            Dark = 1,
            Normal = 2,
            Bright = 3,
            VeryBright = 4
        }
        #endregion
    }
}
