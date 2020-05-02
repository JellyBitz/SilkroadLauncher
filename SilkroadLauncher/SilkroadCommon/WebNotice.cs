namespace SilkroadCommon
{
    public class WebNotice
    {
        public string Subject { get; internal set; }
        public string Article { get; internal set; }
        public ushort Year { get; internal set; }
        public ushort Month { get; internal set; }
        public ushort Day { get; internal set; }
        public ushort Hour { get; internal set; }
        public ushort Minute { get; internal set; }
        public ushort Second { get; internal set; }
        public uint MicroSecond { get; internal set; }
    }
}
