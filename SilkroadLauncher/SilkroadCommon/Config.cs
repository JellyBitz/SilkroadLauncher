using System;
namespace SilkroadLauncher.SilkroadCommon
{
    /// <summary>
    /// SilkCfg.dat file structure
    /// </summary>
    public class Config
    {
        public uint Version { get; set; } = 3;
        public uint unkUint01 { get; set; } = 1;
        public WindowResoltuion Resoltuion { get; set; } = new WindowResoltuion(800, 600);
        public Graphic GraphicType01 { get; set; } = Graphic.Middle;
        public Brightness BrightnessType { get; set; } = Brightness.Normal;
        public byte unkByte01 { get; set; } = 1;
        public bool EnabledSound { get; set; } = true;
        public byte unkByte02 { get; set; } = 0;
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
        public class WindowResoltuion
        {
            public uint Width { get; }
            public uint Height { get; }
            public WindowResoltuion(uint Width, uint Height)
            {
                this.Width = Width;
                this.Height = Height;
            }
            public override string ToString()
            {
                return Width + "x" + Height;
            }
        }
    }
}
