using SilkroadLauncher.Utility;
using SilkroadSecurityAPI;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SilkroadLauncher.Network
{
    /// <summary>
    /// Class to handle the client session. Receiving synchronized and sending asynchronized.
    /// </summary>
    public class Session
    {
        #region Private Members
        private Socket m_Socket;
        private TransferBuffer m_Buffer;
        private Security m_Security;
        /// <summary>
        /// Packet handlers by opcode
        /// </summary>
        private Dictionary<ushort, List<PacketHandler>> m_PacketHandlers;
        /// <summary>
        /// Used to synchronize the incoming packets
        /// </summary>
        private readonly object lockHandlers = new object();
        #endregion

        /// <summary>
        /// Called when the connection is lost for some reason.
        /// </summary>
        public event EventHandler Disconnect;
        public Session()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Buffer = new TransferBuffer(8192);
            m_Security = new Security();

            m_PacketHandlers = new Dictionary<ushort, List<PacketHandler>>();
        }

        #region Public Methods
        /// <summary>
        /// Starts the connetion to the server, Return success.
        /// </summary>
        public bool Start(string Host, ushort Port, int MilisecondsTimeout)
        {
            try
            {
                m_Socket.Connect(Host, Port, MilisecondsTimeout);
            }
            catch
            {
                return false;
            }
            // Make sure is connected
            if (!m_Socket.Connected)
                return false;

            // Start reading asynchronized
            ReceivingAsync();

            return true;
        }
        /// <summary>
        /// Stops the connetion.
        /// </summary>
        public void Stop()
        {
            m_Socket?.Close();
        }
        /// <summary>
        /// Add handler to opcode
        /// </summary>
        public void AddHandler(ushort Opcode, PacketHandler Handler)
        {
            // Check if the opcode has some subscriber
            if (!m_PacketHandlers.TryGetValue(Opcode, out List<PacketHandler> handlers))
            {
                // Create the subscribers list for this opcode
                handlers = new List<PacketHandler>();
                m_PacketHandlers[Opcode] = handlers;
            }
            // Add the new handler
            handlers.Add(Handler);
        }
        /// <summary>
        /// Remove handler to opcode
        /// </summary>
        public void RemoveHandler(ushort Opcode, PacketHandler Handler = null)
        {
            // Remove all
            if (Handler == null)
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
                    if (handlers.Count == 0)
                        m_PacketHandlers.Remove(Opcode);
                }
            }
        }
        /// <summary>
        /// Send packet to the server.
        /// </summary>
        public void Send(Packet packet)
        {
            m_Security.Send(packet);
        }
        #endregion

        #region Private Members
        private void ReceivingAsync()
        {
            try
            {
                m_Socket.BeginReceive(m_Buffer.Buffer, 0, m_Buffer.Buffer.Length, SocketFlags.None,
                    (_asyncResult) =>
                    {
                        try
                        {
                            int recvCount = m_Socket.EndReceive(_asyncResult);

                            if (recvCount == 0)
                                throw new Exception("Remote connection has been lost");

                            if (recvCount > 0)
                            {

                                m_Security.Recv(m_Buffer.Buffer, 0, recvCount);

                                OnReceivedPackets(m_Security.TransferIncoming());

                                SendingAsync();
                            }

                            ReceivingAsync();
                        }
                        catch
                        {
                            OnDisconnect();
                        }

                    }, null);
            }
            catch
            {
                OnDisconnect();
            }
        }
        private void SendingAsync()
        {
            try
            {
                var buffers = m_Security.TransferOutgoing();
                if (buffers != null)
                {
                    foreach (var kvp in buffers)
                    {
                        m_Socket.BeginSend(kvp.Key.Buffer, kvp.Key.Offset, kvp.Key.Size, SocketFlags.None,
                            (_asyncResult) =>
                            {
                                m_Socket.EndSend(_asyncResult);
                            }, null);
                    }
                }
            }
            catch
            {
                OnDisconnect();
            }
        }

        private void OnDisconnect()
        {
            if (m_Socket != null)
            {
                m_Socket.Close();
                m_Socket = null;
            }
            Disconnect?.Invoke(this, EventArgs.Empty);
        }
        private void OnReceivedPackets(List<Packet> Packets)
        {
            // Just in case
            if (Packets == null)
                return;
            
            foreach (var p in Packets)
            {
                System.Diagnostics.Debug.WriteLine("[" + p.Opcode.ToString("X2") + "]");

                if (m_PacketHandlers.TryGetValue(p.Opcode, out List<PacketHandler> handlers))
                {
                    // lock it to read it by sequence!
                    lock (lockHandlers)
                    {
                        // Execute every packet handler
                        foreach (var handler in handlers)
                        {
                            handler.Execute(p, this);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
