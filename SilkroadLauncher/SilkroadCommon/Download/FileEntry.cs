namespace SilkroadCommon.Download
{
    public class FileEntry
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public uint Size { get; set; }
        public bool ImportToPk2 { get; set; }
    }
}