using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SilkroadLauncher
{
    public class ServerDivisionVM : BaseViewModel
    {
        #region Private Members
        private long mPing = 0;
        private List<string> mHosts;
        #endregion

        #region Constructor
        public ServerDivisionVM(string name, List<string> hosts)
        {
            Name = name;
            mHosts = hosts;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Host name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Lowest ping to the hosts
        /// </summary>
        public long Ping
        {
            get => mPing;
            set
            {
                mPing = value;
                OnPropertyChanged(nameof(Ping));
                OnPropertyChanged(nameof(PingString));
                OnPropertyChanged(nameof(PingStatus));
            }
        }
        public string PingString => (Ping > 9999 ? "+9999" : Ping.ToString()) + " ms";
        /// <summary>
        /// Gets the host index from ping
        /// </summary>
        public int HostIndex { get; private set; }
        /// <summary>
        /// Get the host address from ping
        /// </summary>
        public string Host => mHosts[HostIndex];
        /// <summary>
        /// Ping advice for non-geek users
        /// </summary>
        public string PingStatus
        {
            get
            {
                if (Ping <= 50)
                    return "Excellent";
                else if (Ping <= 100)
                    return "Good";
                else if (Ping <= 250)
                    return "Average";
                return "Bad";
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check and update the lowest ping to host available
        /// </summary>
        public async Task CalculatePing(int port)
        {
            mPing = long.MaxValue;
            HostIndex = 0;
            for (var i = 0; i < mHosts.Count; i++)
            {
                var ping = await CheckPing(mHosts[i], port);
                if (ping != -1 && ping < mPing)
                {
                    mPing = ping;
                    HostIndex = i;
                }
            }
            Ping = mPing;
        }
        #endregion

        #region Private Helpers
        private async Task<int> CheckPing(string host, int port, int timeoutMs = 9999)
        {
            try
            {
                var timeBegins = DateTime.Now;

                // Resolve IP address
                IPAddress[] addresses = await Dns.GetHostAddressesAsync(host);
                if (addresses.Length == 0)
                    return timeoutMs;

                var remoteEP = new IPEndPoint(addresses[0], port);

                using (var socket = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    var connectTask = socket.ConnectAsync(remoteEP);
                    // Start a delay task acting as our timeout
                    var delayTask = Task.Delay(timeoutMs);
                    var completedTask = await Task.WhenAny(connectTask, delayTask);

                    // If the delay task finished first, it timed out
                    if (completedTask == delayTask)
                    {
                        // Crucial: Close the socket immediately to abort the background connection attempt
                        socket.Close();
                        return timeoutMs;
                    }

                    // If we reach here, connectTask completed. We await it to propagate any errors (like connection refused)
                    await connectTask;
                    return (int)(DateTime.Now - timeBegins).TotalMilliseconds;
                }
            }
            catch (Exception)
            {
                // Catches SocketException (connection refused, host unreachable, etc.)
                return timeoutMs;
            }
        }
        #endregion
    }
}
