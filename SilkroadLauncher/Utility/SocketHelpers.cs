using System;
using System.Diagnostics;
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
        /// <param name="timeout">Timeout to wait for connection at miliseconds</param>
        /// <param name="elapsed">Time elapsed from connection</param>
        public static void Connect(this Socket socket, string host, int port, long timeout, out long elapsed)
        {
            var timeoutWatch = Stopwatch.StartNew();
            // wait result
            var result = socket.BeginConnect(host, port, null, null);
            result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout), true);
            if (socket.Connected)
            {
                socket.EndConnect(result);
                elapsed = timeoutWatch.ElapsedMilliseconds;
            }
            else
            {
                socket.Close();
                // Connection timed out
                throw new SocketException(10060);
            }
        }
    }
}