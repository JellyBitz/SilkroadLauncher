using System.Net;
using System.Net.Sockets;
namespace SilkroadLauncher.Utility
{
    public static class SocketHelpers
    {
        /// <summary>
        /// Connects the specified socket
        /// </summary>
        /// <param name="host">Host/IP to connect</param>
        /// <param name="port">Port</param>
        /// <param name="timeout">Timeout to wait for connection</param>
        public static void Connect(this Socket socket, string host, int port, int timeout)
        {
            var result = socket.BeginConnect(host, port, null, null);
            result.AsyncWaitHandle.WaitOne(timeout, true);
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