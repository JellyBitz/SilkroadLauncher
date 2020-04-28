using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilkroadSecurityAPI;

namespace SilkroadLauncher.Network
{
    public class PacketHandler
    {
        private Action<Packet,Security> m_Action;
        public PacketHandler(Action<Packet, Security> Action)
        {
            m_Action = Action;
        }
        public void Execute(Packet packet, Security security)
        {
            m_Action.Invoke(packet, security);
        }
    }
}
