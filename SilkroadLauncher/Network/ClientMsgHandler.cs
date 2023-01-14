using SilkroadSecurityAPI;

using System;

namespace SilkroadLauncher.Network
{
    public delegate void ClientMsgHandler(object sender, ClientMsgEventArgs e);
    public class ClientMsgEventArgs : EventArgs
    {
        public Packet Packet { get; }
        internal ClientMsgEventArgs(Packet Packet)
        {
            this.Packet = Packet;
        }
    }
}
