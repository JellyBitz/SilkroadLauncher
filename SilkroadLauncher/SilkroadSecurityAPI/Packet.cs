using System;
using System.Text;
using System.IO;
namespace SilkroadSecurityAPI
{
    public class Packet
    {
        private PacketWriter m_writer;
        private PacketReader m_reader;
        private bool m_locked;
        byte[] m_reader_bytes;
        object m_lock;

        public ushort Opcode { get; }
        public bool Encrypted { get; }
        public bool Massive { get; }
        public Packet(Packet rhs)
        {
            lock (rhs.m_lock)
            {
                m_lock = new object();

                Opcode = rhs.Opcode;
                Encrypted = rhs.Encrypted;
                Massive = rhs.Massive;

                m_locked = rhs.m_locked;
                if (!m_locked)
                {
                    m_writer = new PacketWriter();
                    m_reader = null;
                    m_reader_bytes = null;
                    m_writer.Write(rhs.m_writer.GetBytes());
                }
                else
                {
                    m_writer = null;
                    m_reader_bytes = rhs.m_reader_bytes;
                    m_reader = new PacketReader(m_reader_bytes);
                }
            }
        }
        public Packet(ushort opcode)
        {
            m_lock = new object();
            Opcode = opcode;
            Encrypted = false;
            Massive = false;
            m_writer = new PacketWriter();
            m_reader = null;
            m_reader_bytes = null;
        }
        public Packet(ushort opcode, bool encrypted)
        {
            m_lock = new object();
            Opcode = opcode;
            Encrypted = encrypted;
            Massive = false;
            m_writer = new PacketWriter();
            m_reader = null;
            m_reader_bytes = null;
        }
        public Packet(ushort opcode, bool encrypted, bool massive)
        {
            if (encrypted && massive)
            {
                throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
            }
            m_lock = new object();
            Opcode = opcode;
            Encrypted = encrypted;
            Massive = massive;
            m_writer = new PacketWriter();
            m_reader = null;
            m_reader_bytes = null;
        }
        public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes)
        {
            if (encrypted && massive)
            {
                throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
            }
            m_lock = new object();
            Opcode = opcode;
            Encrypted = encrypted;
            Massive = massive;
            m_writer = new PacketWriter();
            m_writer.Write(bytes);
            m_reader = null;
            m_reader_bytes = null;
        }
        public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes, int offset, int length)
        {
            if (encrypted && massive)
            {
                throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
            }
            m_lock = new object();
            Opcode = opcode;
            Encrypted = encrypted;
            Massive = massive;
            m_writer = new PacketWriter();
            m_writer.Write(bytes, offset, length);
            m_reader = null;
            m_reader_bytes = null;
        }

        public byte[] GetBytes()
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    return m_reader_bytes;
                }
                return m_writer.GetBytes();
            }
        }

        public void Lock()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    m_reader_bytes = m_writer.GetBytes();
                    m_reader = new PacketReader(m_reader_bytes);
                    m_writer.Close();
                    m_writer = null;
                    m_locked = true;
                }
            }
        }

        public long SeekRead(long offset, SeekOrigin orgin)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot SeekRead on an unlocked Packet.");
                }
                return m_reader.BaseStream.Seek(offset, orgin);
            }
        }
        public int RemainingRead()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot SeekRead on an unlocked Packet.");
                }
                return (int)(m_reader.BaseStream.Length - m_reader.BaseStream.Position);
            }
        }
        public byte ReadByte()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadByte();
            }
        }
        public sbyte ReadSByte()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadSByte();
            }
        }
        public ushort ReadUShort()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadUInt16();
            }
        }
        public short ReadShort()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadInt16();
            }
        }
        public uint ReadUInt()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadUInt32();
            }
        }
        public int ReadInt()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadInt32();
            }
        }
        public ulong ReadULong()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadUInt64();
            }
        }
        public long ReadLong()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadInt64();
            }
        }
        public float ReadFloat()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadSingle();
            }
        }
        public double ReadDouble()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                return m_reader.ReadDouble();
            }
        }
        public string ReadAscii()
        {
            return ReadAscii(1252);
        }
        public string ReadAscii(int codepage)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }

                UInt16 length = m_reader.ReadUInt16();
                byte[] bytes = m_reader.ReadBytes(length);

                return Encoding.GetEncoding(codepage).GetString(bytes);
            }
        }
        public string ReadUnicode()
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }

                UInt16 length = m_reader.ReadUInt16();
                byte[] bytes = m_reader.ReadBytes(length * 2);

                return Encoding.Unicode.GetString(bytes);
            }
        }
        public byte[] ReadByteArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                byte[] values = new byte[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadByte();
                }
                return values;
            }
        }
        public sbyte[] ReadSByteArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                sbyte[] values = new sbyte[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadSByte();
                }
                return values;
            }
        }
        public ushort[] ReadUShortArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                ushort[] values = new ushort[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadUInt16();
                }
                return values;
            }
        }
        public short[] ReadShortArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                short[] values = new short[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadInt16();
                }
                return values;
            }
        }
        public uint[] ReadUIntArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                uint[] values = new uint[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadUInt32();
                }
                return values;
            }
        }
        public int[] ReadIntArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                int[] values = new int[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadInt32();
                }
                return values;
            }
        }
        public ulong[] ReadULongArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                ulong[] values = new ulong[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadUInt64();
                }
                return values;
            }
        }
        public long[] ReadLongArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                long[] values = new long[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadInt64();
                }
                return values;
            }
        }
        public float[] ReadFloatArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                float[] values = new float[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadSingle();
                }
                return values;
            }
        }
        public double[] ReadDoubleArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                double[] values = new double[count];
                for (int x = 0; x < count; ++x)
                {
                    values[x] = m_reader.ReadDouble();
                }
                return values;
            }
        }
        public string[] ReadAsciiArray(int count)
        {
            return ReadAsciiArray(1252);
        }
        public string[] ReadAsciiArray(int codepage, int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                string[] values = new string[count];
                for (int x = 0; x < count; ++x)
                {
                    ushort length = m_reader.ReadUInt16();
                    byte[] bytes = m_reader.ReadBytes(length);
                    values[x] = Encoding.UTF7.GetString(bytes);
                }
                return values;
            }
        }
        public string[] ReadUnicodeArray(int count)
        {
            lock (m_lock)
            {
                if (!m_locked)
                {
                    throw new Exception("Cannot Read from an unlocked Packet.");
                }
                string[] values = new string[count];
                for (int x = 0; x < count; ++x)
                {
                    ushort length = m_reader.ReadUInt16();
                    byte[] bytes = m_reader.ReadBytes(length * 2);
                    values[x] = Encoding.Unicode.GetString(bytes);
                }
                return values;
            }
        }

        public long SeekWrite(long offset, SeekOrigin orgin)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot SeekWrite on a locked Packet.");
                }
                return m_writer.BaseStream.Seek(offset, orgin);
            }
        }

        public void WriteByte(byte value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteInt8(sbyte value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteUShort(UInt16 value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteShort(Int16 value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteUInt(uint value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteInt(int value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteULong(ulong value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteLong(long value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteFloat(float value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteDouble(double value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(value);
            }
        }
        public void WriteAscii(string value)
        {
            WriteAscii(value, 1252);
        }
        public void WriteAscii(string value, int code_page)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }

                byte[] codepage_bytes = Encoding.GetEncoding(code_page).GetBytes(value);
                string utf7_value = Encoding.UTF7.GetString(codepage_bytes);
                byte[] bytes = Encoding.Default.GetBytes(utf7_value);

                m_writer.Write((ushort)bytes.Length);
                m_writer.Write(bytes);
            }
        }
        public void WriteUnicode(string value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }

                byte[] bytes = Encoding.Unicode.GetBytes(value);

                m_writer.Write((ushort)value.ToString().Length);
                m_writer.Write(bytes);
            }
        }

        public void WriteByte(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write((byte)(Convert.ToUInt64(value) & 0xFF));
            }
        }
        public void WriteSByte(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write((sbyte)(Convert.ToInt64(value) & 0xFF));
            }
        }
        public void WriteUShort(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write((ushort)(Convert.ToUInt64(value) & 0xFFFF));
            }
        }
        public void WriteShort(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write((ushort)(Convert.ToInt64(value) & 0xFFFF));
            }
        }
        public void WriteUInt(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write((uint)(Convert.ToUInt64(value) & 0xFFFFFFFF));
            }
        }
        public void WriteInt(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write((int)(Convert.ToInt64(value) & 0xFFFFFFFF));
            }
        }
        public void WriteULong(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(Convert.ToUInt64(value));
            }
        }
        public void WriteLong(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(Convert.ToInt64(value));
            }
        }
        public void WriteFloat(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(Convert.ToSingle(value));
            }
        }
        public void WriteDouble(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                m_writer.Write(Convert.ToDouble(value));
            }
        }
        public void WriteAscii(object value)
        {
            WriteAscii(value, 1252);
        }
        public void WriteAscii(object value, int code_page)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }

                byte[] codepage_bytes = Encoding.GetEncoding(code_page).GetBytes(value.ToString());
                string utf7_value = Encoding.UTF7.GetString(codepage_bytes);
                byte[] bytes = Encoding.Default.GetBytes(utf7_value);

                m_writer.Write((ushort)bytes.Length);
                m_writer.Write(bytes);
            }
        }
        public void WriteUnicode(object value)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }

                byte[] bytes = Encoding.Unicode.GetBytes(value.ToString());

                m_writer.Write((ushort)value.ToString().Length);
                m_writer.Write(bytes);
            }
        }

        public void WriteByteArray(byte[] values)
        {
            if (m_locked)
            {
                throw new Exception("Cannot Write to a locked Packet.");
            }
            m_writer.Write(values);
        }
        public void WriteByteArray(byte[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteUShortArray(UInt16[] values)
        {
            WriteUShortArray(values, 0, values.Length);
        }
        public void WriteUShortArray(UInt16[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteShortArray(Int16[] values)
        {
            WriteShortArray(values, 0, values.Length);
        }
        public void WriteShortArray(Int16[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteUIntArray(UInt32[] values)
        {
            WriteUIntArray(values, 0, values.Length);
        }
        public void WriteUIntArray(UInt32[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteIntArray(Int32[] values)
        {
            WriteIntArray(values, 0, values.Length);
        }
        public void WriteIntArray(Int32[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteULongArray(UInt64[] values)
        {
            WriteULongArray(values, 0, values.Length);
        }
        public void WriteULongArray(UInt64[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteLongArray(Int64[] values)
        {
            WriteLongArray(values, 0, values.Length);
        }
        public void WriteLongArray(Int64[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteFloatArray(float[] values)
        {
            WriteFloatArray(values, 0, values.Length);
        }
        public void WriteFloatArray(float[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteDoubleArray(double[] values)
        {
            WriteDoubleArray(values, 0, values.Length);
        }
        public void WriteDoubleArray(double[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    m_writer.Write(values[x]);
                }
            }
        }
        public void WriteAsciiArray(String[] values, int codepage)
        {
            WriteAsciiArray(values, 0, values.Length, codepage);
        }
        public void WriteAsciiArray(String[] values, int index, int count, int codepage)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteAscii(values[x], codepage);
                }
            }
        }
        public void WriteAsciiArray(String[] values)
        {
            WriteAsciiArray(values, 0, values.Length, 1252);
        }
        public void WriteAsciiArray(String[] values, int index, int count)
        {
            WriteAsciiArray(values, index, count, 1252);
        }
        public void WriteUnicodeArray(String[] values)
        {
            WriteUnicodeArray(values, 0, values.Length);
        }
        public void WriteUnicodeArray(String[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteUnicode(values[x]);
                }
            }
        }

        public void WriteByteArray(object[] values)
        {
            WriteByteArray(values, 0, values.Length);
        }
        public void WriteByteArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteByte(values[x]);
                }
            }
        }
        public void WriteInt8Array(object[] values)
        {
            WriteInt8Array(values, 0, values.Length);
        }
        public void WriteInt8Array(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteSByte(values[x]);
                }
            }
        }
        public void WriteUShortArray(object[] values)
        {
            WriteUShortArray(values, 0, values.Length);
        }
        public void WriteUShortArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteUShort(values[x]);
                }
            }
        }
        public void WriteShortArray(object[] values)
        {
            WriteShortArray(values, 0, values.Length);
        }
        public void WriteShortArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteShort(values[x]);
                }
            }
        }
        public void WriteUIntArray(object[] values)
        {
            WriteUIntArray(values, 0, values.Length);
        }
        public void WriteUIntArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteUInt(values[x]);
                }
            }
        }
        public void WriteIntArray(object[] values)
        {
            WriteIntArray(values, 0, values.Length);
        }
        public void WriteIntArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteInt(values[x]);
                }
            }
        }
        public void WriteULongArray(object[] values)
        {
            WriteULongArray(values, 0, values.Length);
        }
        public void WriteULongArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteULong(values[x]);
                }
            }
        }
        public void WriteLongArray(object[] values)
        {
            WriteLongArray(values, 0, values.Length);
        }
        public void WriteLongArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteLong(values[x]);
                }
            }
        }
        public void WriteFloatArray(object[] values)
        {
            WriteFloatArray(values, 0, values.Length);
        }
        public void WriteFloatArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteFloat(values[x]);
                }
            }
        }
        public void WriteDoubleArray(object[] values)
        {
            WriteDoubleArray(values, 0, values.Length);
        }
        public void WriteDoubleArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteDouble(values[x]);
                }
            }
        }
        public void WriteAsciiArray(object[] values, int codepage)
        {
            WriteAsciiArray(values, 0, values.Length, codepage);
        }
        public void WriteAsciiArray(object[] values, int index, int count, int codepage)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteAscii(values[x].ToString(), codepage);
                }
            }
        }
        public void WriteAsciiArray(object[] values)
        {
            WriteAsciiArray(values, 0, values.Length, 1252);
        }
        public void WriteAsciiArray(object[] values, int index, int count)
        {
            WriteAsciiArray(values, index, count, 1252);
        }
        public void WriteUnicodeArray(object[] values)
        {
            WriteUnicodeArray(values, 0, values.Length);
        }
        public void WriteUnicodeArray(object[] values, int index, int count)
        {
            lock (m_lock)
            {
                if (m_locked)
                {
                    throw new Exception("Cannot Write to a locked Packet.");
                }
                for (int x = index; x < index + count; ++x)
                {
                    WriteUnicode(values[x].ToString());
                }
            }
        }
    }
}
