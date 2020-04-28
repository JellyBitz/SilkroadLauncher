using SilkroadSecurityAPI;
using System.Collections.Generic;
namespace SilkroadLauncher.Network
{
    public static class PacketManager
    {
        private static Dictionary<ushort, List<PacketHandler>> m_PacketHandlers = new Dictionary<ushort, List<PacketHandler>>();

        public static void AddHandler(ushort Opcode,PacketHandler Handler)
        {
            // Check if the opcode has some subscriber
            if (!m_PacketHandlers.TryGetValue(Opcode,out List<PacketHandler> handlers))
            {
                // Create the subscribers list for this opcode
                handlers = new List<PacketHandler>();
                m_PacketHandlers[Opcode] = handlers;
            }
            // Add the new handler
            handlers.Add(Handler);
        }
        public static void RemoveHandler(ushort Opcode, PacketHandler Handler = null)
        {
            // Remove all
            if(Handler == null)
            {
                if (m_PacketHandlers.ContainsKey(Opcode))
                    m_PacketHandlers.Remove(Opcode);
            }
            // Remove specific handler
            else
            {
                // Check if the opcode has handlers
                if (m_PacketHandlers.TryGetValue(Opcode, out List<PacketHandler> handlers))
                {
                    for (int i = 0; i < handlers.Count; i++)
                    {
                        // Remove handler
                        if (Handler == handlers[i])
                            handlers.RemoveAt(i--);
                    }
                    // Clean opcode event
                    if(handlers.Count == 0)
                        m_PacketHandlers.Remove(Opcode);
                }
            }
        }
        public static void OnReceivedPackets(List<Packet> Packets,Security security)
        {
            foreach (var packet in Packets)
            {
                if (m_PacketHandlers.TryGetValue(packet.Opcode, out List<PacketHandler> handlers))
                {
                    // Execute every packet handler
                    foreach (var handler in handlers)
                    {
                        handler.Execute(packet, security);
                    }
                }
            }
        }
    }
}
