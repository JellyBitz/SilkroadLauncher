using Pk2ReaderAPI;
using SilkroadSecurityAPI;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SilkroadLauncher.Utility
{
    /// <summary>
    /// Extensions helper for the Pk2 reader
    /// </summary>
    public static class Pk2ReaderHelpers
    {
        /// <summary>
        /// Try to get the Silkroad version from Pk2
        /// </summary>
        /// <returns>Return success</returns>
        public static bool TryGetVersion(this Pk2Reader Pk2Reader,out uint Version)
        {
            try
            {
                // Localize the file and prepare to read it 
                Stream data = Pk2Reader.GetFileStream("SV.T");
                BinaryReader buffer = new BinaryReader(data, Encoding.ASCII);

                // Reading the encrypted file
                int versionLength = buffer.ReadInt32();
                byte[] versionBuffer = buffer.ReadBytes(versionLength);

                // Initialize the blowfish to decrypt the file
                Blowfish bf = new Blowfish();
                bf.Initialize(Encoding.ASCII.GetBytes("SILKROADVERSION"), 0, versionLength);

                // Decrypting
                versionBuffer = bf.Decode(versionBuffer);

                // Only four starting bytes contains the numbers
                Version = uint.Parse(Encoding.ASCII.GetString(versionBuffer, 0, 4));

                // Success
                return true;
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
        public static bool TryGetLocale(this Pk2Reader Pk2Reader,out byte Locale)
        {
            try
            {
                // Localize the file and prepare to read it 
                Stream data = Pk2Reader.GetFileStream("DivisionInfo.txt");
                BinaryReader buffer = new BinaryReader(data, Encoding.ASCII);

                // Read first byte only
                Locale = buffer.ReadByte();

                // Success
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
        public static bool TryGetDivisionInfo(this Pk2Reader Pk2Reader, out Dictionary<string, List<string>> DivisionInfo)
        {
            try
            {
                // Localize the file and prepare to read it 
                Stream data = Pk2Reader.GetFileStream("DivisionInfo.txt");
                BinaryReader buffer = new BinaryReader(data, Encoding.ASCII);

                // initialize
                DivisionInfo = new Dictionary<string, List<string>>();

                // Ignore locale byte
                buffer.ReadByte();

                // Reading all divitions
                byte divisionCount = buffer.ReadByte();
                for (byte i = 0; i < divisionCount; i++)
                {
                    // Division Name
                    string name = new string(buffer.ReadChars(buffer.ReadInt32()));
                    // skip value (0)
                    buffer.ReadByte();

                    // Division hosts
                    byte hostCount = buffer.ReadByte();

                    List<string> hosts = new List<string>(hostCount);
                    for (byte j = 0; j < hostCount; j++)
                    {
                        // host address
                        string host = new string(buffer.ReadChars(buffer.ReadInt32()));
                        // skip value (0)
                        buffer.ReadByte();

                        // Add host
                        hosts.Add(host);
                    }

                    // Add/overwrite division
                    DivisionInfo[name] = hosts;
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
        public static bool TryGetGateport(this Pk2Reader Pk2Reader,out ushort Gateport)
        {
            try
            {
                // Localize the file and prepare to read it 
                string data = Pk2Reader.GetFileText("Gateport.txt");

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
    }
}
