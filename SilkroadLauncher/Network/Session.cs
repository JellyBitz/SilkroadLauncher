using SilkroadLauncher.Utility;
using SilkroadSecurityAPI;
using System.Net.Sockets;

namespace SilkroadLauncher.Network
{
    public class Session
    {
        private Socket _clientSocket;
        private byte[] _clientRecvBuffer;
        private Security _clientSecurity;
        public Session()
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientRecvBuffer = new byte[4096];
            _clientSecurity = new Security();
        }

        public bool StartSession(string Host,ushort Port,int MilisecondsTimeout)
        {
            try
            {
                _clientSocket.Connect(Host, Port, MilisecondsTimeout);
            }
            catch
            {
                return false;
            }
            if(!_clientSocket.Connected)
                return false;

            BeginReceiving();
            return true;
        }

        private void BeginReceiving()
        {
            try
            {
                _clientSocket.BeginReceive(_clientRecvBuffer, 0, _clientRecvBuffer.Length, SocketFlags.None,
                    (iar) =>
                    {
                        try
                        {
                            int recvCount = _clientSocket.EndReceive(iar);

                            if (recvCount == 0)
                                _clientSocket.Close();

                            if (recvCount > 0)
                            {

                                _clientSecurity.Recv(_clientRecvBuffer, 0, recvCount);

                                PacketManager.OnReceivedPackets(_clientSecurity.TransferIncoming(),_clientSecurity);

                                BeginSending();
                            }

                        }
                        catch
                        {
                            _clientSocket.Close();
                        }
                        BeginReceiving();
                    }, null);
            }
            catch
            {
                _clientSocket.Close();
            }
        }

        private void BeginSending()
        {
            try
            {
                var buffers = _clientSecurity.TransferOutgoing();
                if (buffers != null)
                {
                    foreach (var kvp in buffers)
                    {
                        _clientSocket.BeginSend(kvp.Key.Buffer, kvp.Key.Offset, kvp.Key.Size, SocketFlags.None,
                            (iar) =>
                            {
                                try
                                {
                                    _clientSocket.EndSend(iar);
                                }
                                catch
                                {
                                    _clientSocket.Close();
                                }
                            }, null);
                    }
                }
            }
            catch
            {
                _clientSocket.Close();
            }
        }
    }
}
