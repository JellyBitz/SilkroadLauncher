namespace Pk2ReaderAPI
{
	public class Pk2File
	{
		public string Name { get; set; }
		public long Position { get; set; }
        public uint Size { get; set; }
        public Pk2Folder ParentFolder { get; set; }
        /// <summary>
		/// Gets the file extension
		/// </summary>
        public string GetExtension()
        {
            int offset = Name.LastIndexOf('.') + 1;
            return Name.Substring(offset);
        }
    }
}