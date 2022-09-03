using System;
using SilkroadSecurityAPI;

namespace SilkroadLauncher.Network
{
    public delegate void SessionPacketHandler(object sender, SessionPacketEventArgs e);
    public class SessionPacketEventArgs : EventArgs
    {
        public Packet Packet { get; }
        internal SessionPacketEventArgs(Packet Packet)
        {
            this.Packet = Packet;
        }
    }
}
