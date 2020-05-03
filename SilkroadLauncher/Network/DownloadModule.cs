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
        public static void Server_Ready(Packet p, Session s)
        {
            System.Diagnostics.Debug.WriteLine("Server_Ready");

            // Create a temporary directory
            Directory.CreateDirectory("Temp");

            // Init file holder
            m_FileStream = new FileStream("Temp\\"+ DownloadFiles[0].ID,FileMode.Create,FileAccess.Write);

            // Request the first file
            Packet response = new Packet(Opcode.CLIENT_FILE_REQUEST, false);
            response.WriteUInt(DownloadFiles[0].ID);
            response.WriteUInt(0);
            s.Send(response);
        }
        public static void Server_FileChunk(Packet p, Session s)
        {
            System.Diagnostics.Debug.WriteLine("Server_FileChunk");

            // Continue adding bytes to the file
            byte[] buffer = p.GetBytes();
            m_FileStream.WriteAsync(buffer, 0, buffer.Length);

            Globals.LauncherViewModel.UpdatingBytesDownloading += (uint)buffer.Length;
        }
        public static void Server_FileCompleted(Packet p, Session s)
        {
            byte result = p.ReadByte();
            System.Diagnostics.Debug.WriteLine($"Server_FileCompleted: {DownloadFiles[0].ID} - Result:{result}");

            // Close the file
            m_FileStream.Close();
            m_FileStream.Dispose();

            // Process the file
            if (DownloadFiles[0].ToBePacked)
            {
                // Get the Pk2 to open
                int pk2FileNameLength = DownloadFiles[0].Path.IndexOf("\\");
                string pk2FileName = DownloadFiles[0].Path.Remove(pk2FileNameLength);
                // Open the Pk2 and insert the file
                if (Pk2Writer.Open(pk2FileName, Globals.BlowfishKey))
                {
                    if (Pk2Writer.ImportFile(DownloadFiles[0].Path.Substring(pk2FileNameLength + 1) + "\\" + DownloadFiles[0].Name, "Temp\\" + DownloadFiles[0].ID))
                        System.Diagnostics.Debug.WriteLine($"File {DownloadFiles[0].Name} imported into the Pk2");
                    // Close the Pk2
                    Pk2Writer.Close();
                }
                   
                // Delete the file
                File.Delete("Temp\\" + DownloadFiles[0].ID);
            }
            else
            {
                // Just move it from Temp to the folder required
                Directory.CreateDirectory(DownloadFiles[0].Path);
                File.Move("Temp\\" + DownloadFiles[0].ID, DownloadFiles[0].Path+ "\\" + DownloadFiles[0].Name);
            }
            
            // Remove the file and delete it from Temp
            DownloadFiles.RemoveAt(0);
            
            // Continue protocol
            if (DownloadFiles.Count > 0)
            {
                // Init file holder
                m_FileStream = new FileStream("Temp\\" + DownloadFiles[0].ID, FileMode.Create, FileAccess.Write);

                // Request the next file
                Packet response = new Packet(Opcode.CLIENT_FILE_REQUEST, false);
                response.WriteUInt(DownloadFiles[0].ID);
                response.WriteUInt(0);
                s.Send(response);
            }
            else
            {
                // Dispose writer
                Pk2Writer.Deinitialize();

                // Update done
                Globals.LauncherViewModel.Version += 1;
                Globals.LauncherViewModel.IsUpdating = false;
                System.Diagnostics.Debug.WriteLine($"Download finished!");
                Globals.LauncherViewModel.CanStartGame = true;

                // Stop downloader
                s.Stop();
            }
        }
    }
}