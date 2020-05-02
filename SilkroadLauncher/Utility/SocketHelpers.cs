using System.Net;
using System.Net.Sockets;
namespace SilkroadLauncher.Utility
{
    public static class SocketHelpers
    {
        /// <summary>
        /// Connects the specified socket
        /// </summary>
        /// <param name="host">The host/IP to connect</param>
        /// <param name="port">The port</param>
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
                // Connection timed out.
                throw new SocketException(10060);
            }
        }
    }
}