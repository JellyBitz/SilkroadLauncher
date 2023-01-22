using System;
namespace SilkroadLauncher.SilkroadCommon
{
    /// <summary>
    /// SilkCfg.dat file structure
    /// </summary>
    public class SilkCfg
    {
        public uint Version { get; set; } = 3;
        public uint unkUint01 { get; set; } = 1;
        public WindowResolution Resolution { get; set; } = new WindowResolution(800, 600);
        public Graphic GraphicType { get; set; } = Graphic.Middle;
        public Brightness BrightnessType { get; set; } = Brightness.Normal;
        public byte unkByte01 { get; set; } = 1;
        public bool IsSoundEnabled { get; set; } = true;
        public byte unkByte02 { get; set; } = 0;
        public byte unkByte03 { get; set; } = 1;
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
        /// <summary>
        /// Game window resolution
        /// </summary>
        public class WindowResolution
        {
            public uint Width { get; }
            public uint Height { get; }
            public WindowResolution(uint Width, uint Height)
            {
                this.Width = Width;
                this.Height = Height;
            }
            #region Enum Type Reference
            public override string ToString()
            {
                return Width + " * " + Height;
            }
            public override bool Equals(object obj)
            {
                return (obj is WindowResolution wr && wr.Width == Width && wr.Height == Height);
            }
            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }
            #endregion
        }
    }
}
