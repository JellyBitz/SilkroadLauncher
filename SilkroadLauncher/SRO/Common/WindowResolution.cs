namespace SRO.Common
{
    /// <summary>
    /// Window resolution
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
        public override string ToString() => Width + " * " + Height;
        public override bool Equals(object obj) => obj is WindowResolution wr && wr.Width == Width && wr.Height == Height;
        public override int GetHashCode() => ToString().GetHashCode();
    }
}
