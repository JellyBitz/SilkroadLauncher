using SilkroadSecurityAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Pk2ReaderAPI
{
	public class Pk2Reader
	{
        #region Private Members
        private Blowfish m_Blowfish = new Blowfish();
        private FileStream m_FileStream;
        private Pk2Folder m_RootFolder;
        private Dictionary<string, Pk2Folder> m_Folders = new Dictionary<string, Pk2Folder>();
        private Dictionary<string, Pk2File> m_Files = new Dictionary<string, Pk2File>();
        /// <summary>
        /// Used to normalize the files and folder names without risk
        /// </summary>
        private readonly CultureInfo m_EnglishCulture = new CultureInfo("en-US", false);
        #endregion

        #region Public Properties
        public sPk2Header Header { get; }
        /// <summary>
        /// Gets the byte size of the file
        /// </summary>
        public long Size { get; }
        /// <summary>
        /// Key used to initialize the blowfish
        /// </summary>
        public byte[] Key { get; }
        /// <summary>
        /// Key used to initialize the blowfish at string format
        /// </summary>
        public string ASCIIKey { get; private set; }
        /// <summary>
        /// Path to the pk2 file loaded
        /// </summary>
        public string FullPath { get; }
        /// <summary>
        /// Gets all pk2 files found
        /// </summary>
        public List<Pk2File> Files { get { return new List<Pk2File>(m_Files.Values); } }
        /// <summary>
        /// Gets all pk2 folders found
        /// </summary>
        public List<Pk2Folder> Folders { get { return new List<Pk2Folder>(m_Folders.Values); } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates and load all entries from the Pk2 file
        /// </summary>
        /// <param name="FullPath">Path to the file</param>
        /// <param name="BlowfishKey">Key used to decrypt the file</param>
        public Pk2Reader(string FullPath, string BlowfishKey)
		{
			if (!File.Exists(FullPath))
				throw new FileNotFoundException("Pk2 file not found!");

            // Normalize path
            this.FullPath = Path.GetFullPath(FullPath);

            // Generate key from string
            Key = GenerateFinalBlowfishKey(BlowfishKey);
            ASCIIKey = BlowfishKey;
            
            // Open file
            m_FileStream = new FileStream(this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Size = m_FileStream.Length;

            // Initialize the reader pointer
            m_Blowfish.Initialize(Key);
            BinaryReader reader = new BinaryReader(m_FileStream);
            Header = (sPk2Header)BufferToStruct(reader.ReadBytes(256), typeof(sPk2Header));

            // Initialize root folder
            m_RootFolder = new Pk2Folder { Name = Path.GetDirectoryName(this.FullPath) };

            // Check if the Pk2 file is loaded correctly
            try
            {
                // Reads stream
                Read(reader.BaseStream.Position, m_RootFolder, string.Empty);
            }
            catch(Exception ex)
            {
                // The only reason to fail is a wrong blowfish key or differents Pk2 structures
                throw new FormatException("The Pk2 structure cannot match with the blowfish key.", ex);
            }
		}
        #endregion

        #region Private Helpers
        private static byte[] GenerateFinalBlowfishKey(string ascii_key)
        {
            // Using the default base key
            return GenerateFinalBlowfishKey(ascii_key, new byte[] { 0x03, 0xF8, 0xE4, 0x44, 0x88, 0x99, 0x3F, 0x64, 0xFE, 0x35 });
        }
        private static byte[] GenerateFinalBlowfishKey(string ascii_key, byte[] base_key)
        {
            byte ascii_key_length = (byte)ascii_key.Length;

            // Max count of 56 key bytes
            if (ascii_key_length > 56)
            {
                ascii_key_length = 56;
            }

            // Get bytes from ascii
            byte[] a_key = Encoding.ASCII.GetBytes(ascii_key);

            // This is the Silkroad base key used in all versions
            byte[] b_key = new byte[56];

            // Copy key to array to keep the b_key at 56 bytes. b_key has to be bigger than a_key
            // to be able to xor every index of a_key.
            Array.ConstrainedCopy(base_key, 0, b_key, 0, base_key.Length);

            // Their key modification algorithm for the final blowfish key
            byte[] bf_key = new byte[ascii_key_length];
            for (byte x = 0; x < ascii_key_length; ++x)
            {
                bf_key[x] = (byte)(a_key[x] ^ b_key[x]);
            }

            return bf_key;
        }
        /// <summary>
        /// Reads Pk2 block structure from the position specified and save all data into the Folder
        /// </summary>
        private void Read(long Position,Pk2Folder CurrentFolder, string ParentPath)
		{
            // Set cursor position in the stream
			BinaryReader reader = new BinaryReader(m_FileStream);
			reader.BaseStream.Position = Position;

            // Keep a list with all folders from this block to add it to subfolders
            List<Pk2Folder> subfolders = new List<Pk2Folder>();

            // Read pk2 block
            sPk2EntryBlock entryBlock = (sPk2EntryBlock)BufferToStruct(m_Blowfish.Decode(reader.ReadBytes(Marshal.SizeOf(typeof(sPk2EntryBlock)))), typeof(sPk2EntryBlock));
			for (int i = 0; i < 20; i++)
			{
                // Entry
                sPk2Entry entry = entryBlock.Entries[i];

                // Check entry type
				switch (entry.Type)
                {
                    // Null Entry
                    case 0:
						break;
                    // Folder
                    case 1:
                        // Check if is not a parent/root folder
						if (entry.Name != "." && entry.Name != "..")
						{
                            Pk2Folder folder = new Pk2Folder()
                            {
                                Name = entry.Name,
                                Position = BitConverter.ToInt64(entry.g_Position, 0)
                            };
                            // Add subfolder
                            subfolders.Add(folder);

                            // Save dictionary reference
                            m_Folders[ParentPath + entry.Name.ToUpper(m_EnglishCulture)] = folder;
                        }
						break;
                    // File
                    case 2:
                        Pk2File file = new Pk2File
                        {
                            Position = entry.Position,
                            Name = entry.Name,
                            Size = entry.Size,
                            ParentFolder = CurrentFolder
                        };
                        // Add files to the current folder
                        CurrentFolder.Files.Add(file);

                        // Save dictionary reference
                        m_Files[ParentPath+entry.Name.ToUpper(m_EnglishCulture)] = file;
						break;
				}
			}
            // Read the next pk2 block chain
			if (entryBlock.Entries[19].NextChain != 0)
				Read(entryBlock.Entries[19].NextChain,CurrentFolder, ParentPath);
            
            // Add subfolders to the current folder
            CurrentFolder.SubFolders.AddRange(subfolders);

            // Continue reading folder by folder
            foreach(var f in subfolders)
                Read(f.Position,f, ParentPath + f.Name.ToUpper(m_EnglishCulture) + "\\");
		}
        #endregion

        #region Public Methods
		/// <summary>
		/// Gets all the root files.
		/// </summary>
		public List<Pk2File> GetRootFiles()
		{
			return m_RootFolder.Files;
		}
		/// <summary>
		/// Gets all the root folders.
		/// </summary>
		public List<Pk2Folder> GetRootFolders()
		{
			return m_RootFolder.SubFolders;
		}
        /// <summary>
        /// Gets the Pk2 folder info
        /// </summary>
        /// <param name="FolderPath">Path to the location inside Pk2</param>
		public Pk2Folder GetFolder(string FolderPath)
		{
            if (FolderPath == "")
                return m_RootFolder;

			// Normalize key format
			FolderPath = FolderPath.ToUpper(m_EnglishCulture).Replace("/", "\\");
			if (FolderPath.EndsWith("\\"))
				FolderPath = FolderPath.Remove(FolderPath.Length - 1);

			Pk2Folder folder = null;
			m_Folders.TryGetValue(FolderPath, out folder);
			return folder;
		}
        
        /// <summary>
        /// Gets the Pk2 file info
        /// </summary>
        /// <param name="Path">Path to the location inside Pk2</param>
		public Pk2File GetFile(string Path)
		{
			if (Path == "")
				return null;

            // Normalize key format
            Path = Path.ToUpper(m_EnglishCulture).Replace("/", "\\");

            Pk2File file = null;
			m_Files.TryGetValue(Path, out file);
            return file;
		}
		/// <summary>
		/// Extract bytes from Pk2 file.
		/// </summary>
		public byte[] GetFileBytes(Pk2File File)
		{
			BinaryReader reader = new BinaryReader(m_FileStream);
			reader.BaseStream.Position = File.Position;
			return reader.ReadBytes((int)File.Size);
		}
		/// <summary>
		/// Extract the bytes from Pk2 path specified.
		/// </summary>
		public byte[] GetFileBytes(string Path)
		{
			BinaryReader reader = new BinaryReader(m_FileStream);
			Pk2File file = GetFile(Path);
			reader.BaseStream.Position = file.Position;
			return reader.ReadBytes((int)file.Size);
		}
		/// <summary>
		/// Extract the stream from the file.
		/// </summary>
		public Stream GetFileStream(Pk2File File)
		{
			return new MemoryStream(GetFileBytes(File));
		}
		/// <summary>
		/// Extract the stream from Pk2 path specified.
		/// </summary>
		public Stream GetFileStream(string Path)
		{
			return new MemoryStream(GetFileBytes(Path));
		}
		/// <summary>
		/// Extract the string text from the file.
		/// </summary>
		public string GetFileText(Pk2File File)
		{
			byte[] tempBuffer = GetFileBytes(File);
			if (tempBuffer != null)
			{
				TextReader txtReader = new StreamReader(new MemoryStream(tempBuffer));
				return txtReader.ReadToEnd();
			}
			return null;
		}
		/// <summary>
		/// Extract the string text from Pk2 path specified.
		/// </summary>
		public string GetFileText(string Path)
		{
			byte[] tempBuffer = GetFileBytes(Path);
			if (tempBuffer != null){
				TextReader txtReader = new StreamReader(new MemoryStream(tempBuffer));
				return txtReader.ReadToEnd();
			}
			return null;
		}
		/// <summary>
		/// Extract a file from the buffer to an output path.
		/// </summary>
		public void ExtractFile(Pk2File File, string OutputPath)
		{
			byte[] data = GetFileBytes(File);
			FileStream stream = new FileStream(OutputPath, FileMode.Create);
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(data);
			stream.Close();
        }
        /// <summary>
        /// Close the Pk2 file if is opened
        /// </summary>
        public void Close()
        {
            if (m_FileStream != null)
            {
                m_FileStream.Close();
                m_FileStream.Dispose();
                m_FileStream = null;
            }
        }
        #endregion
        
        #region Pk2 Structures
        object BufferToStruct(byte[] buffer, Type returnStruct)
		{
			IntPtr pointer = Marshal.AllocHGlobal(buffer.Length);
			Marshal.Copy(buffer, 0, pointer, buffer.Length);
			return Marshal.PtrToStructure(pointer, returnStruct);
		}
		[StructLayout(LayoutKind.Sequential, Size = 256)]
		public struct sPk2Header
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
			public string Name;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public byte[] Version;
			[MarshalAs(UnmanagedType.I1, SizeConst = 1)]
			public byte Encryption;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] Verify;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 205)]
			public byte[] Reserved;

		}
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct sPk2Entry
		{
			[MarshalAs(UnmanagedType.I1)]
			public byte Type; // files are 2, folger are 1, null entries re 0
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
			public string Name;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] AccessTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] CreateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] ModifyTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] g_Position;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			private byte[] m_Size;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			private byte[] m_NextChain;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public byte[] Padding;

			public long NextChain { get { return BitConverter.ToInt64(m_NextChain, 0); } }
			public long Position { get { return BitConverter.ToInt64(g_Position, 0); } }
			public uint Size { get { return BitConverter.ToUInt32(m_Size, 0); } }
		}
		[StructLayout(LayoutKind.Sequential, Size = 2560)]
		public struct sPk2EntryBlock
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
			public sPk2Entry[] Entries;
		}
		#endregion
	}
}