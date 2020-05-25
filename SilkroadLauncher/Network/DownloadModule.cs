using Pk2WriterAPI;
using SilkroadCommon.Download;
using SilkroadSecurityAPI;
using System.Collections.Generic;
using System.IO;

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
            SERVER_FILE_COMPLETED = 0xA004;
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

        #region Public Methods
        public static void Server_Ready(Packet p, Session s)
        {
            System.Diagnostics.Debug.WriteLine("Server_Ready");

            // Create a temporary directory to allocate the file
            Directory.CreateDirectory("Temp");

            // Create file download from server
            RequestFileDownload(s);
        }
        public static void Server_FileChunk(Packet p, Session s)
        {
            System.Diagnostics.Debug.WriteLine("Server_FileChunk");

            // Continue adding bytes to the file
            byte[] buffer = p.GetBytes();
            m_FileStream.WriteAsync(buffer, 0, buffer.Length);

            LauncherViewModel.Instance.UpdatingBytesDownloading += (uint)buffer.Length;
        }
        public static void Server_FileCompleted(Packet p, Session s)
        {
            byte result = p.ReadByte();

            // File downloaded
            var file = DownloadFiles[0];

            System.Diagnostics.Debug.WriteLine($"Server_FileCompleted: {file.ID} - Result:{result}");

            // Close the file
            m_FileStream.Close();
            m_FileStream.Dispose();

            // Process the file
            if (file.ImportToPk2)
            {
                // Get the Pk2 to open
                int pk2FileNameLength = file.Path.IndexOf("\\");
                string pk2FileName = file.Path.Remove(pk2FileNameLength);
                // Open the Pk2 and insert the file
                if (Pk2Writer.Open(pk2FileName, LauncherSettings.CLIENT_BLOWFISH_KEY))
                {
                    if (Pk2Writer.ImportFile(file.Path.Substring(pk2FileNameLength + 1) + "\\" + file.Name, "Temp\\" + file.ID))
                        System.Diagnostics.Debug.WriteLine($"File {file.Name} imported into the Pk2");
                    // Close the Pk2
                    Pk2Writer.Close();
                }
                // Delete the file
                File.Delete("Temp\\" + file.ID);
            }
            else
            {
                // Create directory if doesn't exists
                string dir = Path.GetDirectoryName(file.Path);
                if (dir != string.Empty && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                
                // Check if is Launcher to process at the end
                if (!(dir == string.Empty && file.Name == System.Reflection.Assembly.GetEntryAssembly().CodeBase))
                {
                    // Just move it from Temp to the folder required
                    File.Move("Temp\\" + file.ID, file.Path + "\\" + file.Name);
                }
            }
            
            // Remove the file and delete it from Temp
            DownloadFiles.RemoveAt(0);
            
            // Continue protocol
            if (DownloadFiles.Count > 0)
            {
                RequestFileDownload(s);
            }
            else
            {
                LauncherViewModel.Instance.IsUpdating = false;
                System.Diagnostics.Debug.WriteLine($"Download finished!");

                // Dispose writer
                Pk2Writer.Deinitialize();

                // Process Launcher if exists
                if (File.Exists("Temp\\"+ System.Reflection.Assembly.GetEntryAssembly().CodeBase))
                {
                    // Replace launcher required
                    StartReplacer();
                }
                else
                {
                    // Update done, set new version
                    LauncherViewModel.Instance.Version = DownloadVersion;
                    LauncherViewModel.Instance.CanStartGame = true;

                    // Stop connection
                    s.Stop();
                }
            }
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Request a server file to be downloaded inmediatly
        /// </summary>
        private static void RequestFileDownload(Session s)
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
        }
        /// <summary>
        /// Replaces the current executable with the new one downloaded
        /// </summary>
        private static void StartReplacer()
        {
            var launcherFilename = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
            var launcherPath = Path.GetFullPath(launcherFilename);
            // Move my current executable to temp
            File.Move(launcherPath, "Temp\\"+ launcherFilename+".bkp");
            // Move the new launcher to the folder
            File.Move(launcherPath, "Temp\\" + launcherFilename);
            // Run the new launcher
            System.Diagnostics.Process.Start(launcherPath);
            // Close this one
            LauncherViewModel.Instance.CommandClose.Execute(null);
        }
        #endregion
    }
}