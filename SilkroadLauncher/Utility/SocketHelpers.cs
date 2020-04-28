using System.Net;
using System.Net.Sockets;
namespace SilkroadLauncher.Utility
{
    public static class SocketHelpers
    {
        /// <summary>
        /// Connects the specified socket
        /// </summary>
        /// <param name="socket">The socket</param>
        /// <param name="endpoint">The IP endpoint</param>
        /// <param name="timeout">The timeout</param>
        public static void Connect(this Socket socket, string host, int port, int milisecondsTimeout)
        {
            var result = socket.BeginConnect(host, port, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(milisecondsTimeout, true);
            if (socket.Connected)
            {
                socket.EndConnect(result);
            }
            else
            {
                socket.Close();
                throw new SocketException(10060); // Connection timed out.
            }
        }
    }
}