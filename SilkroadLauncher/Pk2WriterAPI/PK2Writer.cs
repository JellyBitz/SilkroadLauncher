using System.Runtime.InteropServices;

namespace Pk2ReaderAPI
{
    public class Pk2Writer
    {
        /// <summary>
        /// Sets up GfxFileManager.DLL for PK2 operations. This function must be called first.
        /// <para>
        /// - <code>bool Initialize(const char * gfxDllFilename);</code>
        /// </para>
        /// </summary>
        [DllImport("Pk2Writer.dll", EntryPoint = "_Initialize@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool Initialize(string gfxDllFilename);
        /// <summary>
        /// Cleans up GfxFileManager.DLL. This function must be called before the
        /// program exits and after Close if a PK2 file was opened.
        /// <para>
        /// - <code>bool Deinitialize();</code>
        /// </para>
        /// </summary>
        [DllImport("Pk2Writer.dll", EntryPoint = "_Deinitialize@0", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool Deinitialize();
        /// <summary>
        /// Opens a PK2 file for writing.
        /// <para>
        /// As example: "169841" - For official sro, "\x32\x30\x30\x39\xC4\xEA" - for zszc, swsro
        /// </para>
        /// <para>
        /// Refer to this guide:
        /// elitepvpers.de/forum/sro-guides-templates/612789-guide-finding-pk2-blowfish-key-5-easy-steps.html
        /// </para>
        /// <para>
        /// - <code>bool Open(const char * pk2Filename, char * accessKey, unsigned char accessKeyLen);</code>
        /// </para>
        /// </summary>
        [DllImport("Pk2Writer.dll", EntryPoint = "_Open@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool Open(string pk2Filename, string accessKey, int accessKeyLen);
        /// <summary>
        /// Closes an opened PK2 file. This function must be called before the program exits and before Deinitialize is called.
        /// <para>
        /// - <code>bool Close();</code>
        /// </para>
        /// </summary>
        [DllImport("Pk2Writer.dll", EntryPoint = "_Close@0", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool Close();
        /// <summary>
        /// Imports a file to the PK2.
        /// <para>
        /// - <code>bool ImportFile(const char * entryFilename, const char * inputFilename);</code>
        /// </para>
        /// </summary>
        /// <param name="entryFilename">The full path to the file into the PK2</param>
        [DllImport("Pk2Writer.dll", EntryPoint = "_ImportFile@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool ImportFile(string entryFilename, string inputFilename);
    }
}