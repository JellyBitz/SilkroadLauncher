using SRO.PK2API;
using SRO.PK2API.Security;

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace SilkroadLauncher.Utility
{
    /// <summary>
    /// Extensions helper for the Pk2 reader
    /// </summary>
    public static class Pk2StreamHelpers
    {
        /// <summary>
        /// Try to get the Silkroad version from Pk2
        /// </summary>
        /// <returns>Return success</returns>
        public static bool TryGetVersion(this Pk2Stream pk2Stream, out uint Version)
        {
            try
            {
                // Localize the file and prepare to read it
                using (var ms = new MemoryStream(pk2Stream.GetFile("SV.T").GetContent()))
                using (var br = new BinaryReader(ms))
                {
                    // Reading the encrypted file
                    int versionLength = br.ReadInt32();
                    byte[] versionBuffer = br.ReadBytes(versionLength);

                    // Initialize the blowfish to decrypt the file
                    Blowfish bf = new Blowfish();
                    bf.Initialize(Encoding.ASCII.GetBytes("SILKROADVERSION"), 0, 8);

                    // Decrypting
                    versionBuffer = bf.Decode(versionBuffer);

                    // Only four starting bytes contains the numbers
                    Version = uint.Parse(Encoding.ASCII.GetString(versionBuffer, 0, 4));

                    // Success
                    return true;
                };
            }
            catch
            {
                Version = uint.MinValue;
                return false;
            }
        }
        /// <summary>
        /// Try to get the file localization type
        /// </summary>
        /// <returns>Return success</returns>
        public static bool TryGetLocale(this Pk2Stream pk2Stream, out byte Locale)
        {
            try
            {
                // Read first byte from file
                Locale = pk2Stream.GetFile("DivisionInfo.txt").GetContent()[0];
                return true;
            }
            catch
            {
                Locale = byte.MinValue;
                return false;
            }
        }
        /// <summary>
        /// Try to get the gateway list by division names
        /// </summary>
        /// <returns>Return success</returns>
        public static bool TryGetDivisionInfo(this Pk2Stream pk2Stream, out Dictionary<string, List<string>> DivisionInfo)
        {
            try
            {
                // Localize the file and prepare to read it

                using (var ms = new MemoryStream(pk2Stream.GetFile("DivisionInfo.txt").GetContent()))
                using (var br = new BinaryReader(ms, Encoding.GetEncoding(949)))
                {
                    // initialize
                    DivisionInfo = new Dictionary<string, List<string>>();

                    // Skip locale byte
                    br.BaseStream.Seek(1, SeekOrigin.Begin);

                    // Reading all divitions
                    byte divisionCount = br.ReadByte();
                    for (byte i = 0; i < divisionCount; i++)
                    {
                        // Division Name
                        string name = new string(br.ReadChars(br.ReadInt32()));
                        // skip value (0)
                        br.ReadByte();

                        // Division hosts
                        byte hostCount = br.ReadByte();

                        List<string> hosts = new List<string>(hostCount);
                        for (byte j = 0; j < hostCount; j++)
                        {
                            // host address
                            string host = new string(br.ReadChars(br.ReadInt32()));
                            // skip value (0)
                            br.ReadByte();

                            // Add host
                            hosts.Add(host);
                        }

                        // Add/overwrite division
                        DivisionInfo[name] = hosts;
                    }
                }
                // Success
                return true;
            }
            catch
            {
                DivisionInfo = null;
                return false;
            }
        }
        /// <summary>
        /// Try to get the port available to connect to the server
        /// </summary>
        /// <returns>Return success</returns>
        public static bool TryGetGateport(this Pk2Stream pk2Stream, out ushort Gateport)
        {
            try
            {
                // Localize the file and prepare to read it 
                string data = pk2Stream.GetFileText("Gateport.txt");

                // The file contains the port only
                Gateport = ushort.Parse(data.Trim());

                // Success
                return true;
            }
            catch
            {
                Gateport = ushort.MinValue;
                return false;
            }
        }
        /// <summary>
        /// Gets the file as string. UTF-8 as default.
        /// </summary>
        public static string GetFileText(this Pk2Stream pk2Stream, string path, int codepage = 65001)
        {
            var f = pk2Stream.GetFile(path) ?? throw new FileNotFoundException("Pk2 file not found", path);
            return Encoding.GetEncoding(codepage).GetString(f.GetContent());
        }
        /// <summary>
        /// Gets image from path
        /// </summary>
        public static BitmapImage GetImage(this Pk2Stream pk2Stream, string path)
        {
            var f = pk2Stream.GetFile(path) ?? throw new FileNotFoundException("Pk2 file not found", path);
            var bm = new BitmapImage();
            // copy from stream
            bm.BeginInit();
            bm.StreamSource = new MemoryStream(f.GetContent());
            bm.EndInit();
            // return img
            return bm;
        }
    }
}
