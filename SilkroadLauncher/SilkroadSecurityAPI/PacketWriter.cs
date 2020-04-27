using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SilkroadSecurityAPI
{
    internal class PacketWriter : BinaryWriter
    {
        MemoryStream m_ms;

        public PacketWriter()
        {
            m_ms = new MemoryStream();
            this.OutStream = m_ms;
        }

        public byte[] GetBytes()
        {
            return m_ms.ToArray();
        }
    }
}
