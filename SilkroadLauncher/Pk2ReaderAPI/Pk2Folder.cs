using System.Collections.Generic;
namespace Pk2ReaderAPI
{
	public class Pk2Folder
	{
		public string Name { get; set; }
        public long Position { get; set; }
        public List<Pk2File> Files { get; set; } = new List<Pk2File>();
        public List<Pk2Folder> SubFolders { get; set; } = new List<Pk2Folder>();
    }
}
