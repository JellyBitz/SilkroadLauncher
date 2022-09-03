using SilkroadLauncher.Utility;
using SilkroadSecurityAPI;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SilkroadLauncher.Network
{
    /// <summary>
    /// Class to handle the client session on asynchronized state
    /// </summary>
    public class Session
    {
        #region Private Members
        private Socket m_Socket = null;
        private TransferBuffer m_Buffer;
        private Security m_Security;
        /// <summary>
        /// Packet handlers by opcode
        /// </summary>
        private readonly Dictionary<ushort, List<SessionPacketHandler>> m_PacketHandlers = new Dictionary<ushort, List<SessionPacketHandler>>();
        #endregion

        #region Constructor
        public Session() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the connetion to the server, Return success.
        /// </summary>
        public bool Start(string Host, ushort Port, int MilisecondsTimeout)
        {
            try
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.Connect(Host, Port, MilisecondsTimeout);
                m_Buffer = new TransferBuffer(8192);
                m_Security = new Security();
            }
            catch(Exception ex)
            {
                _OnDisconnect(ex);
                return false;
            }
            // Make sure is connected
            if (!IsConnected())
                return false;

            // Start reading asynchronized
            BeginReceiveAsync();

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
        /// Check if the socket is connected
        /// </summary>
        public bool IsConnected()
        {
            try
            {
                return !(m_Socket.Poll(1, SelectMode.SelectRead) && m_Socket.Available == 0);
            }
            catch { return false; }
        }
        /// <summary>
        /// Register the handler to the opcode
        /// </summary>
        public void RegisterHandler(ushort Opcode, SessionPacketHandler Handler)
        {
            // Check if the opcode has some subscriber
            if (!m_PacketHandlers.TryGetValue(Opcode, out List<SessionPacketHandler> handlers))
            {
                // Create the subscribers list for this opcode
                handlers = new List<SessionPacketHandler>();
                m_PacketHandlers[Opcode] = handlers;
            }
            // Add the new handler
            handlers.Add(Handler);
        }
        /// <summary>
        /// Delete the handler to the opcode. If handler is not specified, will delete all handler from opcode.
        /// </summary>
        public void DeleteHandler(ushort Opcode, SessionPacketHandler Handler = null)
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
                if (m_PacketHandlers.TryGetValue(Opcode, out List<SessionPacketHandler> handlers))
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
        /// Send packet to the server
        /// </summary>
        public void Send(Packet packet)
        {
            m_Security.Send(packet);
        }
        #endregion

        #region Private Members
        private void BeginReceiveAsync()
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

                                BeginSendAsync();
                            }

                            BeginReceiveAsync();
                        }
                        catch(Exception ex)
                        {
                            _OnDisconnect(ex);
                        }

                    }, null);
            }
            catch(Exception ex)
            {
                _OnDisconnect(ex);
            }
        }
        private void BeginSendAsync()
        {
            try
            {
                var buffers = m_Security.TransferOutgoing();
                if (buffers != null)
                {
                    foreach (var kvp in buffers)
                    {
                        m_Socket.BeginSend(kvp.Key.Buffer, kvp.Key.Offset, kvp.Key.Size, SocketFlags.None, (_asyncResult) =>
                        {
                            m_Socket.EndSend(_asyncResult);
                        }, null);
                    }
                }
            }
            catch (Exception ex)
            {
                _OnDisconnect(ex);
            }
        }
        private void OnReceivedPackets(List<Packet> Packets)
        {
            // Just in case
            if (Packets == null)
                return;
            
            foreach (var p in Packets)
            {
                System.Diagnostics.Debug.WriteLine("[" + p.Opcode.ToString("X2") + "]");

                if (m_PacketHandlers.TryGetValue(p.Opcode, out List<SessionPacketHandler> handlers))
                {
                    // Execute every packet handler
                    foreach (var handler in handlers)
                        handler.Invoke(this,new SessionPacketEventArgs(p));
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when the connection is lost
        /// </summary>
        public event DisconnectEventHandler OnDisconnect;
        public delegate void DisconnectEventHandler(object sender, DisconnectEventArgs e);
        public class DisconnectEventArgs : EventArgs
        {
            public Exception Exception {get; }
            public DisconnectEventArgs(Exception ex)
            {
                Exception = ex;
            }
        }
        private void _OnDisconnect(Exception ex)
        {
            if (m_Socket != null)
            {
                m_Socket.Close();
                m_Socket = null;
            }
            OnDisconnect?.Invoke(this, new DisconnectEventArgs(ex));
        }
        #endregion
    }
}
