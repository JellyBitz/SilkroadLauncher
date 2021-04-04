using System;
using SilkroadSecurityAPI;

namespace SilkroadLauncher.Network
{
    public class PacketHandler
    {
        private Action<Packet, Session> m_Action;
        public PacketHandler(Action<Packet, Session> Action)
        {
            m_Action = Action;
        }
        public void Execute(Packet packet, Session session)
        {
            m_Action.Invoke(packet, session);
        }
    }
}
