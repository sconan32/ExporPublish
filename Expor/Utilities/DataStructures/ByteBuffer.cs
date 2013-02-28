using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace Socona.Expor.Utilities.DataStructures
{
    public sealed class ByteBuffer
    {
        private byte[] buffer;
        private int pos;
      //  private int __length;	// __length is only valid if > pos, otherwise pos is the current length
        private MemoryStream ms;
        //  private int limit;

        internal ByteBuffer(int initialCapacity)
        {
            // buffer = new byte[initialCapacity];
            buffer = null;
        //    __length = -1;
            ms = new MemoryStream(initialCapacity);
        }

        private ByteBuffer(byte[] wrap, int length)
        {
            //this.buffer = wrap;
            //this.pos = length;
            ms = new MemoryStream(wrap, length, wrap.Length);

        }

        internal long Position
        {
            get { return ms.Position; }
            set
            {

                ms.Position = value;
            }
        }

        internal long Length
        {
            get { return ms.Length; }
        }

        private void Grow(int minGrow)
        {
            //byte[] newbuf = new byte[System.Math.Max(buffer.Length + minGrow, buffer.Length * 2)];
            //Buffer.BlockCopy(buffer, 0, newbuf, 0, buffer.Length);
            //buffer = newbuf;
            ms.Capacity = (int)ms.Length + minGrow;
        }

        public int GetInt32()
        {
            byte[] buf = new byte[4];
            ms.Read(buf, 0, 4);
            return buf[0]
               + (buf[1] << 8)
               + (buf[2] << 16)
               + (buf[3] << 24);
        }
        public byte GetByte()
        {
            return (byte)ms.ReadByte();
        }
        public short GetInt16()
        {
            byte[] buf = new byte[2];
            ms.Read(buf, 0, 2);
            return (short)(buf[0]
               + (buf[1] << 8));
        }
        public long GetInt64()
        {
            byte[] buf = new byte[8];
            ms.Read(buf, 0, 8);
            return buf[0]
               + ((long)buf[1] << 8)
               + ((long)buf[2] << 16)
               + ((long)buf[3] << 24)
               + ((long)buf[4] << 32)
               + ((long)buf[5] << 40)
               + ((long)buf[6] << 48)
               + ((long)buf[7] << 56);
        }
        public float GetSingle()
        {
            byte[] buf = new byte[4];
            ms.Read(buf, 0, 4);
            int bits = buf[0]
               + (buf[1] << 8)
               + (buf[2] << 16)
               + (buf[3] << 24);
            return (float)BitConverter.Int64BitsToDouble(bits);
        }
        public double GetDouble()
        {
            byte[] buf = new byte[8];
            ms.Read(buf, 0, 8);
            long bits = buf[0]
                + ((long)buf[1] << 8)
                + ((long)buf[2] << 16)
                + ((long)buf[3] << 24)
                + ((long)buf[4] << 32)
                + ((long)buf[5] << 40)
                + ((long)buf[6] << 48)
                + ((long)buf[7] << 56);
            return BitConverter.Int64BitsToDouble(bits);
        }
        public byte[] GetBytes(int count)
        {
            byte[] buf = new byte[count];
            ms.Read(buf, 0, count);
            return buf;
        }
        public void GetDoubles(double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = GetDouble();
            }
        }
        public void GetSingles(float[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = GetSingle();
            }
        }
        //public string GetString()
        //{ 
        //    //ms.re
        //}
        //// NOTE this does not advance the position
        //internal int GetInt32AtCurrentPosition()
        //{
        //    return buffer[pos]
        //        + (buffer[pos + 1] << 8)
        //        + (buffer[pos + 2] << 16)
        //        + (buffer[pos + 3] << 24);
        //}

        // NOTE this does not advance the position
        //internal byte GetByteAtCurrentPosition()
        //{
        //    return buffer[pos];
        //}

        internal void Write(byte[] value)
        {
            if (pos + value.Length > buffer.Length)
                Grow(value.Length);
            Buffer.BlockCopy(value, 0, buffer, pos, value.Length);
            pos += value.Length;
        }

        internal void Write(byte value)
        {
            if (pos == buffer.Length)
                Grow(1);
            buffer[pos++] = value;
        }

        internal void Write(sbyte value)
        {
            Write((byte)value);
        }

        internal void Write(ushort value)
        {
            Write((short)value);
        }

        internal void Write(short value)
        {
            if (pos + 2 > buffer.Length)
                Grow(2);
            buffer[pos++] = (byte)value;
            buffer[pos++] = (byte)(value >> 8);
        }

        internal void Write(uint value)
        {
            Write((int)value);
        }

        internal void Write(int value)
        {
            if (pos + 4 > buffer.Length)
                Grow(4);
            buffer[pos++] = (byte)value;
            buffer[pos++] = (byte)(value >> 8);
            buffer[pos++] = (byte)(value >> 16);
            buffer[pos++] = (byte)(value >> 24);
        }

        internal void Write(ulong value)
        {
            Write((long)value);
        }

        internal void Write(long value)
        {
            if (pos + 8 > buffer.Length)
                Grow(8);
            buffer[pos++] = (byte)value;
            buffer[pos++] = (byte)(value >> 8);
            buffer[pos++] = (byte)(value >> 16);
            buffer[pos++] = (byte)(value >> 24);
            buffer[pos++] = (byte)(value >> 32);
            buffer[pos++] = (byte)(value >> 40);
            buffer[pos++] = (byte)(value >> 48);
            buffer[pos++] = (byte)(value >> 56);
        }

        internal void Write(float value)
        {
            Write((int)BitConverter.DoubleToInt64Bits(value));
        }
        public void Write(float[] values)
        {
            foreach (double d in values)
            {
                Write(d);
            }
        }
        internal void Write(double value)
        {
            Write(BitConverter.DoubleToInt64Bits(value));
        }
        public void Write(double[] values)
        {
            foreach (double d in values)
            {
                Write(d);
            }
        }
        internal void Write(string str)
        {
            if (str == null)
            {
                Write((byte)0xFF);
            }
            else
            {
                byte[] buf = Encoding.UTF8.GetBytes(str);
                WriteCompressedInt(buf.Length);
                Write(buf);
            }
        }

        internal void WriteCompressedInt(int value)
        {
            if (value <= 0x7F)
            {
                Write((byte)value);
            }
            else if (value <= 0x3FFF)
            {
                Write((byte)(0x80 | (value >> 8)));
                Write((byte)value);
            }
            else
            {
                Write((byte)(0xC0 | (value >> 24)));
                Write((byte)(value >> 16));
                Write((byte)(value >> 8));
                Write((byte)value);
            }
        }

        internal void Write(ByteBuffer bb)
        {
            if (pos + bb.Length > buffer.Length)
                Grow((int)bb.Length);
            Buffer.BlockCopy(bb.buffer, 0, buffer, pos, (int)bb.Length);
            pos += (int)bb.Length;
        }

        internal void WriteTo(System.IO.Stream stream)
        {
            stream.Write(buffer, 0, (int)this.Length);
        }

        internal void Clear()
        {
            pos = 0;
          //  __length = 0;
        }

        internal void Align(int alignment)
        {
            if (pos + alignment > buffer.Length)
                Grow(alignment);
            int newpos = (pos + alignment - 1) & ~(alignment - 1);
            while (pos < newpos)
                buffer[pos++] = 0;
        }

        //internal void WriteTypeDefOrRefEncoded(int token)
        //{
        //    switch (token >> 24)
        //    {
        //        case TypeDefTable.Index:
        //            WriteCompressedInt((token & 0xFFFFFF) << 2 | 0);
        //            break;
        //        case TypeRefTable.Index:
        //            WriteCompressedInt((token & 0xFFFFFF) << 2 | 1);
        //            break;
        //        case TypeSpecTable.Index:
        //            WriteCompressedInt((token & 0xFFFFFF) << 2 | 2);
        //            break;
        //        default:
        //            throw new InvalidOperationException();
        //    }
        //}

        internal void Write(System.IO.Stream stream)
        {
            const int chunkSize = 8192;
            for (; ; )
            {
                if (pos + chunkSize > buffer.Length)
                    Grow(chunkSize);
                int read = stream.Read(buffer, pos, chunkSize);
                if (read <= 0)
                {
                    break;
                }
                pos += read;
            }
        }

        internal byte[] ToArray()
        {
            byte[] buf = new byte[pos];
            Buffer.BlockCopy(buffer, 0, buf, 0, pos);
            return buf;
        }

        internal static ByteBuffer Wrap(byte[] buf)
        {
            return new ByteBuffer(buf, buf.Length);
        }

        internal static ByteBuffer Wrap(byte[] buf, int length)
        {
            return new ByteBuffer(buf, length);
        }

        internal bool Match(int pos, ByteBuffer bb2, int pos2, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (buffer[pos + i] != bb2.buffer[pos2 + i])
                {
                    return false;
                }
            }
            return true;
        }

        internal int Hash()
        {
            int hash = 0;
            int len = (int)this.Length;
            for (int i = 0; i < len; i++)
            {
                hash *= 37;
                hash ^= buffer[i];
            }
            return hash;
        }

        //internal ByteReader GetBlob(int offset)
        //{
        //    return ByteReader.FromBlob(buffer, offset);
        //}
        public int Remaining { get { return (int)Length - (int)Position; } }

        internal void Patch(int offset, byte b)
        {
            buffer[offset] = b;
        }
    }
}
