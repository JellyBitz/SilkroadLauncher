using System.Collections.Generic;
using System.Net.NetworkInformation;

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
        public void CalculatePing()
        {
            mPing = long.MaxValue;
            HostIndex = 0;
            for(var i = 0; i < mHosts.Count; i++)
            {
                var ping = TestPing(mHosts[i]);
                if(ping != -1 && ping < mPing)
                {
                    mPing = ping;
                    HostIndex = i;
                }
            }
            Ping = mPing;
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Ping to a host ip/address. Returns eco in miliseconds or (-1) if is not possible to reach.
        /// </summary>
        private long TestPing(string host)
        {
            try
            {
                using (var pinger = new Ping())
                {
                    var reply = pinger.Send(host);
                    if (reply.Status == IPStatus.Success)
                        return reply.RoundtripTime;
                }
            }
            catch
            {

            }
            return -1;
        }
        #endregion
    }
}
