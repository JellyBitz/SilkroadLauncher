using SilkroadCommon.Download;
using SilkroadSecurityAPI;
using SRO.PK2API;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SilkroadLauncher.Network
{
    public static class DownloadModule
    {
        public static class Opcode
        {
            public const ushort
            CLIENT_FILE_REQUEST = 0x6004,

            SERVER_READY = 0x6005,
            SERVER_FILE_CHUNK = 0x1001,
            SERVER_FILE_COMPLETED = 0xA004,

            GLOBAL_IDENTIFICATION = 0x2001;
        }
        /// <summary>
        /// The current file being downloaded
        /// </summary>
        private static FileStream m_FileStream;
        /// <summary>
        /// Get the files to be downloaded
        /// </summary>
        public static List<FileEntry> DownloadFiles { get; set; } = new List<FileEntry>();
        /// <summary>
        /// The version currently downloaded
        /// </summary>
        public static uint DownloadVersion { get; set; }
        /// <summary>
        /// Keep open pk2 files being used
        /// </summary>
        private static Dictionary<string, Pk2Stream> mPk2Files = null;
        #region Public Methods
        public static void Server_Ready(object sender, ClientMsgEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DownloadModule::Server_Ready");

            // Set up pk2 files to open them only once
            mPk2Files = new Dictionary<string, Pk2Stream>();

            // Create a temporary directory to allocate the file being downloaded
            Directory.CreateDirectory("Temp");

            // Create file download from server
            RequestFileDownload((Client)sender);
        }
        public static void Server_FileChunk(object sender, ClientMsgEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DownloadModule::Server_FileChunk");

            // Continue adding bytes to the file
            byte[] buffer = e.Packet.GetBytes();
            m_FileStream.Write(buffer, 0, buffer.Length);

            LauncherViewModel.Instance.UpdatingBytesDownloading += (uint)buffer.Length;
            // File being downloaded
            LauncherViewModel.Instance.UpdatingFilePercentage = (int)(m_FileStream.Length * 100L / DownloadFiles[0].Size);
        }
        public static void Server_FileCompleted(object sender, ClientMsgEventArgs e)
        {
            byte result = e.Packet.ReadByte();

            // File downloaded
            var file = DownloadFiles[0];

            System.Diagnostics.Debug.WriteLine($"Server_FileCompleted: {file.ID} - Result:{result}");

            // Close the file
            m_FileStream.Close();
            m_FileStream.Dispose();

            // Process the file
            if (file.ImportToPk2)
            {
                // Get the Pk2 to be open
                var pk2NameIndex = file.Path.IndexOf("\\");
                var pk2Name = pk2NameIndex == -1 ? file.Path : file.Path.Remove(pk2NameIndex);
                var pk2FileName = pk2Name.ToLower() + ".pk2";
                // Open the Pk2 and insert the file
                if (!mPk2Files.TryGetValue(pk2FileName, out var pk2))
                {
                    try
                    {
                        pk2 = new Pk2Stream(pk2FileName, LauncherSettings.CLIENT_BLOWFISH_KEY);
                        mPk2Files[pk2FileName] = pk2;
                    }
                    catch
                    {
                        LauncherViewModel.Instance.ShowMessage("Fatal error opening \"" + pk2Name + "\"!");
                        LauncherViewModel.Instance.Exit();
                    }
                }
                
                DecompressFile("Temp\\" + file.ID);
                
                // Set pk2 path to be used
                var pk2Path = (pk2NameIndex == -1 ? "" : file.Path.Substring(pk2Name.Length + 1) + "\\") + file.Name;
                // Import file
                if (pk2.AddFile(pk2Path, File.ReadAllBytes("Temp\\" + file.ID)))
                {
                    System.Diagnostics.Debug.WriteLine($"File {file.Name} imported into the Pk2");
                }
                else
                {
                    LauncherViewModel.Instance.ShowMessage("Fatal error updating \"" + file.Name + "\" from \"" + pk2Name + "\"!");
                    LauncherViewModel.Instance.Exit();
                }
                // Delete the file
                File.Delete("Temp\\" + file.ID);
            }
            else
            {
                DecompressFile("Temp\\" + file.ID);

                // Check file path
                if (file.Path != string.Empty)
                {
                    // Create directory if doesn't exists
                    if (!Directory.Exists(file.Path))
                        Directory.CreateDirectory(file.Path);
                    // Fix path for easy file moving
                    file.Path += "\\";
                }

                // Check if it's the Launcher to process it at the end
                var launcherFilename = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                if (file.Path == string.Empty && file.Name.Equals(launcherFilename, StringComparison.InvariantCultureIgnoreCase))
                {
                    ForceMovingFile("Temp\\" + file.ID, "Temp\\_" + file.Name);
                }
                else
                {
                    // Move or replace from Temp to the folder required
                    if (!ForceMovingFile("Temp\\" + file.ID, file.Path + file.Name))
                    {
                        LauncherViewModel.Instance.ShowMessage(string.Format(LauncherSettings.MSG_ERR_FILE_UPDATE, file.Name));
                        LauncherViewModel.Instance.Exit();
                    };
                }
            }

            // Remove the file and delete it from Temp
            DownloadFiles.RemoveAt(0);

            // Continue protocol
            if (DownloadFiles.Count > 0)
            {
                RequestFileDownload((Client)sender);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Download finished!");

                // Update SV.T on media.pk2
                UpdateSilkroadVersion();

                // Update done!
                LauncherViewModel.Instance.IsUpdating = false;

                // Close all pk2 streams opened
                foreach (var v in mPk2Files.Values)
                    v.Dispose();

                // Replace the Launcher if exists
                if (File.Exists("Temp\\_" + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)))
                {
                    // Replace launcher required
                    StartReplacer();
                }
                else
                {
                    // Update launcher version
                    LauncherViewModel.Instance.Version = DownloadVersion;
                    LauncherViewModel.Instance.CanStartGame = true;

                    // Stop connection
                    ((Client)sender).Stop();
                }
            }
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Request a server file to be downloaded inmediatly
        /// </summary>
        private static void RequestFileDownload(Client s)
        {
            // Request the first file on list
            var file = DownloadFiles[0];

            // Init file holder
            m_FileStream = new FileStream("Temp\\" + file.ID, FileMode.Create, FileAccess.Write);

            // Request the first file
            Packet response = new Packet(Opcode.CLIENT_FILE_REQUEST, false);
            response.WriteUInt(file.ID);
            response.WriteUInt(0);
            s.Send(response);

            // File being downloaded
            LauncherViewModel.Instance.UpdatingFilePath = file.Path + "\\" + file.Name;
            LauncherViewModel.Instance.UpdatingFilePercentage = 0;
        }
        /// <summary>
        /// Updates the SV.T file with the newer version
        /// </summary>
        private static void UpdateSilkroadVersion()
        {
            // Check if it's already opened otherwise open it
            if (!mPk2Files.TryGetValue(LauncherSettings.CLIENT_MEDIA_PK2_PATH.ToLower(), out var pk2))
                pk2 = new Pk2Stream(LauncherSettings.CLIENT_MEDIA_PK2_PATH, LauncherSettings.CLIENT_BLOWFISH_KEY);

            // Set the new version
            var versionBuffer = Encoding.ASCII.GetBytes(DownloadVersion.ToString());
            Array.Resize(ref versionBuffer, 8); // Set minimum padding required for encoding

            // Init blowfish for encoding
            var bf = new Blowfish();
            bf.Initialize(Encoding.ASCII.GetBytes("SILKROADVERSION"), 0, 8);
            // Encode it and add empty padding as 1024
            versionBuffer = bf.Encode(versionBuffer, 0, versionBuffer.Length);
            Array.Resize(ref versionBuffer, 1024);

            // Overwrite the SV.T file
            var bytes = BitConverter.GetBytes(versionBuffer.Length);
            Array.Resize(ref bytes, bytes.Length + versionBuffer.Length);
            Array.Copy(versionBuffer, 0, bytes, 4, versionBuffer.Length);
            if (pk2.AddFile("SV.T", bytes))
                System.Diagnostics.Debug.WriteLine($"New version ({1000 + DownloadVersion}) created and imported into the Pk2");
        }
        /// <summary>
        /// Replaces the current executable with the new one downloaded
        /// </summary>
        private static void StartReplacer()
        {
            var launcherFilename = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var launcherPath = Path.GetFullPath(launcherFilename);
            // Move my current executable to temp
            ForceMovingFile(launcherPath, "Temp\\" + launcherFilename + ".bkp");
            // Move the new launcher to the folder
            File.Move("Temp\\_" + launcherFilename, launcherPath);
            // Run the new launcher
            System.Diagnostics.Process.Start(launcherPath);
            // Close this one
            LauncherViewModel.Instance.Exit();
        }
        /// <summary>
        /// Move a file. The destination file will be replaced if exists.
        /// </summary>
        private static bool ForceMovingFile(string sourceFileName, string destFileName)
        {
            if (File.Exists(destFileName))
            {
                try
                {
                    File.Delete(destFileName);
                }
                catch (UnauthorizedAccessException)
                {
                    // File is being used probably, just try to move it
                    var newFilePath = Path.GetTempFileName();
                    try
                    {
                        File.Delete(newFilePath);
                        File.Move(destFileName, newFilePath);
                    }
                    catch
                    {
                        /* Give up with this file... */
                        return false;
                    }
                }
            }
            File.Move(sourceFileName, destFileName);
            return true;
        }
        /// <summary>
        /// Decompress the file with zlib
        /// </summary>
        private static void DecompressFile(string Path)
        {
            ForceMovingFile(Path, Path + ".tmp");
            // Create an empty file to decompress (zlib)
            using (FileStream outFileStream = File.Create(Path))
            {
                using (FileStream inFileStream = new FileStream(Path + ".tmp", FileMode.Open, FileAccess.Read))
                {
                    // Read past unknown bytes (4) and zlib header bytes (2)
                    inFileStream.Seek(6, SeekOrigin.Begin);

                    using (DeflateStream zlib = new DeflateStream(inFileStream, CompressionMode.Decompress))
                    {
                        zlib.CopyTo(outFileStream);
                    }
                }
                // Delete decompressed file
                File.Delete(Path + ".tmp");
            }
        }
        #endregion
    }
}