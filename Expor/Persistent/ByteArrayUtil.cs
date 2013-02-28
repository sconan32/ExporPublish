using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Persistent
{

    /**
     * Class with various utilities for manipulating byte arrays.
     * 
     * If you find a reusable copy of this in the Java API, please tell me. Using a
     * {@link java.io.ByteArrayOutputStream} and {@link java.io.DataInputStream}
     * doesn't seem appropriate.
     * 
     * C.f. {@link java.io.DataOutputStream} and
     * {@link java.io.ByteArrayOutputStream}
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     * 
     * @apiviz.composedOf ByteSerializer
     * @apiviz.composedOf ShortSerializer
     * @apiviz.composedOf IntegerSerializer
     * @apiviz.composedOf LongSerializer
     * @apiviz.composedOf FloatSerializer
     * @apiviz.composedOf DoubleSerializer
     * @apiviz.composedOf StringSerializer
     * @apiviz.composedOf VarintSerializer
     */
    public class ByteArrayUtil
    {
        /**
         * Size of a byte in bytes.
         */
        public readonly static int SIZE_BYTE = 1;

        /**
         * Size of a short in bytes.
         */
        public readonly static int SIZE_SHORT = 2;

        /**
         * Size of an integer in bytes.
         */
        public readonly static int SIZE_INT = 4;

        /**
         * Size of a long in bytes.
         */
        public readonly static int SIZE_LONG = 8;

        /**
         * Size of a float in bytes.
         */
        public readonly static int SIZE_FLOAT = 4;

        /**
         * Size of a double in bytes.
         */
        public readonly static int SIZE_DOUBLE = 8;

        /**
         * Write a short to the byte array at the given offset.
         * 
         * @param array Array to write to
         * @param offset Offset to write to
         * @param v data
         * @return number of bytes written
         */
        public static int WriteShort(byte[] array, int offset, int v)
        {
            array[offset + 0] = (byte)(v >> 8);
            array[offset + 1] = (byte)(v >> 0);
            return SIZE_SHORT;
        }

        /**
         * Write an integer to the byte array at the given offset.
         * 
         * @param array Array to write to
         * @param offset Offset to write to
         * @param v data
         * @return number of bytes written
         */
        public static int WriteInt(byte[] array, int offset, int v)
        {
            array[offset + 0] = (byte)(v >> 24);
            array[offset + 1] = (byte)(v >> 16);
            array[offset + 2] = (byte)(v >> 8);
            array[offset + 3] = (byte)(v >> 0);
            return SIZE_INT;
        }

        /**
         * Write a long to the byte array at the given offset.
         * 
         * @param array Array to write to
         * @param offset Offset to write to
         * @param v data
         * @return number of bytes written
         */
        public static int WriteLong(byte[] array, int offset, long v)
        {
            array[offset + 0] = (byte)(v >> 56);
            array[offset + 1] = (byte)(v >> 48);
            array[offset + 2] = (byte)(v >> 40);
            array[offset + 3] = (byte)(v >> 32);
            array[offset + 4] = (byte)(v >> 24);
            array[offset + 5] = (byte)(v >> 16);
            array[offset + 6] = (byte)(v >> 8);
            array[offset + 7] = (byte)(v >> 0);
            return SIZE_LONG;
        }

        /**
         * Write a float to the byte array at the given offset.
         * 
         * @param array Array to write to
         * @param offset Offset to write to
         * @param v data
         * @return number of bytes written
         */
        public static int WriteFloat(byte[] array, int offset, float v)
        {
            return WriteInt(array, offset, (int)BitConverter.DoubleToInt64Bits(v));
        }

        /**
         * Write a double to the byte array at the given offset.
         * 
         * @param array Array to write to
         * @param offset Offset to write to
         * @param v data
         * @return number of bytes written
         */
        public static int WriteDouble(byte[] array, int offset, double v)
        {
            return WriteLong(array, offset, BitConverter.DoubleToInt64Bits(v));
        }

        /**
         * Read a short from the byte array at the given offset.
         * 
         * @param array Array to read from
         * @param offset Offset to read at
         * @return (signed) short
         */
        public static short ReadShort(byte[] array, int offset)
        {
            // First make integers to resolve signed vs. unsigned issues.
            int b0 = array[offset + 0] & 0xFF;
            int b1 = array[offset + 1] & 0xFF;
            return (short)((b0 << 8) + (b1 << 0));
        }

        /**
         * Read an unsigned short from the byte array at the given offset.
         * 
         * @param array Array to read from
         * @param offset Offset to read at
         * @return short
         */
        public static int ReadUnsignedShort(byte[] array, int offset)
        {
            // First make integers to resolve signed vs. unsigned issues.
            int b0 = array[offset + 0] & 0xFF;
            int b1 = array[offset + 1] & 0xFF;
            return ((b0 << 8) + (b1 << 0));
        }

        /**
         * Read an integer from the byte array at the given offset.
         * 
         * @param array Array to read from
         * @param offset Offset to read at
         * @return data
         */
        public static int ReadInt(byte[] array, int offset)
        {
            // First make integers to resolve signed vs. unsigned issues.
            int b0 = array[offset + 0] & 0xFF;
            int b1 = array[offset + 1] & 0xFF;
            int b2 = array[offset + 2] & 0xFF;
            int b3 = array[offset + 3] & 0xFF;
            return ((b0 << 24) + (b1 << 16) + (b2 << 8) + (b3 << 0));
        }

        /**
         * Read a long from the byte array at the given offset.
         * 
         * @param array Array to read from
         * @param offset Offset to read at
         * @return data
         */
        public static long ReadLong(byte[] array, int offset)
        {
            // First make integers to resolve signed vs. unsigned issues.
            long b0 = array[offset + 0];
            long b1 = array[offset + 1] & 0xFF;
            long b2 = array[offset + 2] & 0xFF;
            long b3 = array[offset + 3] & 0xFF;
            long b4 = array[offset + 4] & 0xFF;
            int b5 = array[offset + 5] & 0xFF;
            int b6 = array[offset + 6] & 0xFF;
            int b7 = array[offset + 7] & 0xFF;
            return ((b0 << 56) + (b1 << 48) + (b2 << 40) + (b3 << 32) + (b4 << 24) + (b5 << 16) + (b6 << 8) + (b7 << 0));
        }

        /**
         * Read a float from the byte array at the given offset.
         * 
         * @param array Array to read from
         * @param offset Offset to read at
         * @return data
         */
        public static float ReadFloat(byte[] array, int offset)
        {
            return (float)BitConverter.Int64BitsToDouble(ReadInt(array, offset));
        }

        /**
         * Read a double from the byte array at the given offset.
         * 
         * @param array Array to read from
         * @param offset Offset to read at
         * @return data
         */
        public static double ReadDouble(byte[] array, int offset)
        {
            return BitConverter.Int64BitsToDouble(ReadLong(array, offset));
        }

        /**
         * Serializer for byte objects
         * 
         * @author Erich Schubert
         */
        public class ByteSerializer : IFixedSizeByteBufferSerializer<Byte>
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal ByteSerializer()
                : base()
            {

            }



            public int GetFixedByteSize()
            {
                throw new NotImplementedException();
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Serializer for short objects
         * 
         * @author Erich Schubert
         */
        public class ShortSerializer : IFixedSizeByteBufferSerializer<short>
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal ShortSerializer()
                : base()
            {
            }


            public  short FromByteBuffer(ByteBuffer buffer)
            {
                return buffer.GetInt16();
            }


            public  void ToByteBuffer(ByteBuffer buffer, short obj)
            {
                buffer.Write(obj);
            }


            public  int GetByteSize(short o)
            {
                return GetFixedByteSize();
            }


            public  int GetFixedByteSize()
            {
                return SIZE_SHORT;
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Serializer for integer objects
         * 
         * @author Erich Schubert
         */
        public class IntegerSerializer : IFixedSizeByteBufferSerializer<Int32>
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal IntegerSerializer()
                : base()
            {

            }


            public  Int32 FromByteBuffer(ByteBuffer buffer)
            {
                return buffer.GetInt32();
            }


            public  void toByteBuffer(ByteBuffer buffer, Int32 obj)
            {
                buffer.Write(obj);
            }


            public  int GetByteSize(Int32 o)
            {
                return GetFixedByteSize();
            }


            public  int GetFixedByteSize()
            {
                return SIZE_INT;
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Serializer for long objects
         * 
         * @author Erich Schubert
         */
        public class LongSerializer : IFixedSizeByteBufferSerializer<long>
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal LongSerializer()
                : base()
            {

            }


            public  long FromByteBuffer(ByteBuffer buffer)
            {
                return buffer.GetInt64();
            }


            public  void ToByteBuffer(ByteBuffer buffer, long obj)
            {
                buffer.Write(obj);
            }


            public  int GetByteSize(long o)
            {
                return GetFixedByteSize();
            }


            public  int GetFixedByteSize()
            {
                return SIZE_LONG;
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Serializer for float objects
         * 
         * @author Erich Schubert
         */
        public class FloatSerializer : IFixedSizeByteBufferSerializer<float>
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal FloatSerializer()
                : base()
            {

            }


            public  float FromByteBuffer(ByteBuffer buffer)
            {
                return buffer.GetSingle();
            }


            public  void ToByteBuffer(ByteBuffer buffer, float obj)
            {
                buffer.Write(obj);
            }


            public  int GetByteSize(float o)
            {
                return GetFixedByteSize();
            }


            public  int GetFixedByteSize()
            {
                return SIZE_FLOAT;
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Serializer for double objects
         * 
         * @author Erich Schubert
         */
        public class DoubleSerializer : IFixedSizeByteBufferSerializer<Double>
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal DoubleSerializer()
                : base()
            {

            }


            public  Double FromByteBuffer(ByteBuffer buffer)
            {
                return buffer.GetDouble();
            }


            public  void ToByteBuffer(ByteBuffer buffer, Double obj)
            {
                buffer.Write(obj);
            }


            public  int GetByteSize(Double o)
            {
                return GetFixedByteSize();
            }

            public  int GetFixedByteSize()
            {
                return SIZE_DOUBLE;
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Serializer for String objects
         * 
         * @author Erich Schubert
         */
        public class StringSerializer : IByteBufferSerializer
        {
            /**
             * Character set to use
             */
            Encoding encoding = Encoding.UTF8;
            //Charset charset = Charset.forName("UTF-8");

            /**
             * Encoder
             */
            //CharsetEncoder encoder = charset.newEncoder();
            Encoder encoder = Encoding.UTF8.GetEncoder();

            /**
             * Decoder
             */
            //CharsetDecoder decoder = charset.newDecoder();
            Decoder decoder = Encoding.UTF8.GetDecoder();

            /**
             * Constructor. Protected: use static instance!
             */
            internal StringSerializer()
                : base()
            {

            }


            public  String FromByteBuffer(ByteBuffer buffer)
            {
                int len = buffer.GetInt32();
                // Create and limit a view
                byte[] strbuf = buffer.GetBytes(len);
                //CharBuffer res;
                StringBuilder res = new StringBuilder();
                try
                {
                    res.Append(encoding.GetString(strbuf));
                }
                catch (Exception e)
                {
                    buffer.Position -= len;
                    throw new AbortException("String not representable as UTF-8.", e);

                }
                // TODO: Debug.Assert that the decoding did not yet advance the buffer!

                return res.ToString();
            }


            public  void ToByteBuffer(ByteBuffer buffer, String obj)
            {
                byte[] strbuf;
                try
                {
                    strbuf = encoding.GetBytes(obj);//encoder.encode(CharBuffer.wrap(obj));
                }
                catch (Exception e)
                {

                    throw new AbortException("String not representable as UTF-8.", e);
                }
                buffer.Write(strbuf.Length);
                buffer.Write(strbuf);
            }


            public  int GetByteSize(String obj)
            {
                try
                {
                    return SIZE_INT + encoding.GetByteCount(obj);
                }
                catch (Exception e)
                {
                    throw new AbortException("String not representable as UTF-8.", e);
                }
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Serializer for Integer objects using a variable size encoding
         * 
         * @author Erich Schubert
         */
        public class VarintSerializer : IByteBufferSerializer
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal VarintSerializer()
                : base()
            {

            }


            public  Int32 FromByteBuffer(ByteBuffer buffer)
            {
                return ReadSignedVarint(buffer);
            }


            public  void ToByteBuffer(ByteBuffer buffer, Int32 obj)
            {
                WriteSignedVarint(buffer, obj);
            }


            public  int GetByteSize(Int32 obj)
            {
                return GetSignedVarintSize(obj);
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Static instance.
         */
        public static readonly ByteSerializer BYTE_SERIALIZER = new ByteSerializer();

        /**
         * Static instance.
         */
        public static readonly ShortSerializer SHORT_SERIALIZER = new ShortSerializer();

        /**
         * Static instance.
         */
        public static readonly IntegerSerializer INT_SERIALIZER = new IntegerSerializer();

        /**
         * Static instance.
         */
        public static readonly LongSerializer LONG_SERIALIZER = new LongSerializer();

        /**
         * Static instance.
         */
        public static readonly FloatSerializer FLOAT_SERIALIZER = new FloatSerializer();

        /**
         * Static instance.
         */
        public static readonly DoubleSerializer DOUBLE_SERIALIZER = new DoubleSerializer();

        /**
         * Static instance.
         */
        public static readonly StringSerializer STRING_SERIALIZER = new StringSerializer();

        /**
         * Static instance.
         */
        public static readonly VarintSerializer VARINT_SERIALIZER = new VarintSerializer();

        /**
         * Write an signed integer using a variable-length encoding.
         * 
         * The sign bit is moved to bit 0.
         * 
         * Data is always written in 7-bit little-endian, where the 8th bit is the
         * continuation flag.
         * 
         * @param buffer Buffer to write to
         * @param val number to write
         */
        public static  void WriteSignedVarint(ByteBuffer buffer, int val)
        {
            // Move sign to lowest bit
            WriteUnsignedVarint(buffer, (val << 1) ^ (val >> 31));
        }

        /**
         * Write a signed long using a variable-length encoding.
         * 
         * The sign bit is moved to bit 0.
         * 
         * Data is always written in 7-bit little-endian, where the 8th bit is the
         * continuation flag.
         * 
         * @param buffer Buffer to write to
         * @param val number to write
         */
        public static void WriteSignedVarintLong(ByteBuffer buffer, long val)
        {
            // Move sign to lowest bit
            WriteUnsignedVarintLong(buffer, (val << 1) ^ (val >> 63));
        }

        /**
         * Write an unsigned integer using a variable-length encoding.
         * 
         * Data is always written in 7-bit little-endian, where the 8th bit is the
         * continuation flag.
         * 
         * @param buffer Buffer to write to
         * @param val number to write
         */
        public static void WriteUnsignedVarint(ByteBuffer buffer, int val)
        {
            // Extra bytes have the high bit set
            while ((val & 0x7F) != val)
            {
                buffer.Write((byte)((val & 0x7F) | 0x80));
                val >>= 7;
            }
            // Last byte doesn't have high bit set
            buffer.Write((byte)(val & 0x7F));
        }

        /**
         * Write an unsigned long using a variable-length encoding.
         * 
         * Data is always written in 7-bit little-endian, where the 8th bit is the
         * continuation flag.
         * 
         * Note that for integers, this will result in the same encoding as
         * {@link #writeUnsignedVarint}
         * 
         * @param buffer Buffer to write to
         * @param val number to write
         */
        public static void WriteUnsignedVarintLong(ByteBuffer buffer, long val)
        {
            // Extra bytes have the high bit set
            while ((val & 0x7F) != val)
            {
                buffer.Write((byte)((val & 0x7F) | 0x80));
                val >>= 7;
            }
            // Last byte doesn't have high bit set
            buffer.Write((byte)(val & 0x7F));
        }

        /**
         * Compute the size of the varint encoding for this signed integer
         * 
         * @param val integer to write
         * @return Encoding size of this integer
         */
        public static int GetSignedVarintSize(int val)
        {
            // Move sign to lowest bit
            return GetUnsignedVarintSize((val << 1) ^ (val >> 31));
        }

        /**
         * Compute the size of the varint encoding for this unsigned integer
         * 
         * @param obj integer to write
         * @return Encoding size of this integer
         */
        public static int GetUnsignedVarintSize(int obj)
        {
            int bytes = 1;
            // Extra bytes have the high bit set
            while ((obj & 0x7F) != obj)
            {
                bytes++;
                obj >>= 7;
            }
            return bytes;
        }

        /**
         * Compute the size of the varint encoding for this signed integer
         * 
         * @param val integer to write
         * @return Encoding size of this integer
         */
        public static int GetSignedVarintLongSize(long val)
        {
            // Move sign to lowest bit
            return GetUnsignedVarintLongSize((val << 1) ^ (val >> 31));
        }

        /**
         * Compute the size of the varint encoding for this unsigned integer
         * 
         * @param obj integer to write
         * @return Encoding size of this integer
         */
        public static  int GetUnsignedVarintLongSize(long obj)
        {
            int bytes = 1;
            // Extra bytes have the high bit set
            while ((obj & 0x7F) != obj)
            {
                bytes++;
                obj >>= 7;
            }
            return bytes;
        }

        /**
         * Read a signed integer.
         * 
         * @param buffer Buffer to read from
         * @return Integer value
         */
        public static  int ReadSignedVarint(ByteBuffer buffer)
        {
            int raw = ReadUnsignedVarint(buffer);
            return (raw >> 1) ^ -(raw & 1);
        }

        /**
         * Read an unsigned integer.
         * 
         * @param buffer Buffer to read from
         * @return Integer value
         */
        public static int ReadUnsignedVarint(ByteBuffer buffer)
        {
            int val = 0;
            int bits = 0;
            while (true)
            {
                int data = buffer.GetByte();
                val |= (data & 0x7F) << bits;
                if ((data & 0x80) == 0)
                {
                    return val;
                }
                bits += 7;
                if (bits > 35)
                {
                    throw new AbortException("Variable length quantity is too long for expected integer.");
                }
            }
        }

        /**
         * Read a signed long.
         * 
         * @param buffer Buffer to read from
         * @return long value
         */
        public static long ReadSignedVarintLong(ByteBuffer buffer)
        {
            long raw = ReadUnsignedVarintLong(buffer);
            return (raw >> 1) ^ -(raw & 1);
        }

        /**
         * Read an unsigned long.
         * 
         * @param buffer Buffer to read from
         * @return long value
         */
        public static  long ReadUnsignedVarintLong(ByteBuffer buffer)
        {
            long val = 0;
            int bits = 0;
            while (true)
            {
                int data = buffer.GetByte();
                val |= ((long)data & 0x7F) << bits;
                if ((data & 0x80) == 0)
                {
                    return val;
                }
                bits += 7;
                if (bits > 63)
                {
                    throw new AbortException("Variable length quantity is too long for expected integer.");
                }
            }
        }

        /**
         * Unmap a byte buffer.
         * 
         * @param map Byte buffer to unmap.
         */
        //public static void UnmapByteBuffer( MappedByteBuffer map) {
        //  if(map == null) {
        //    return;
        //  }
        //  map.force();
        //  // This is an ugly hack, but all that Java currently offers.
        //  // See also: http://bugs.sun.com/view_bug.do?bug_id=4724038
        //  AccessController.doPrivileged(new PrivilegedAction<Object>() {
        //    @Override
        //    public Object run() {
        //      try {
        //        Method getCleanerMethod = map.getClass().getMethod("cleaner", new Class[0]);
        //        if(getCleanerMethod == null) {
        //          return null;
        //        }

        //        getCleanerMethod.setAccessible(true);
        //        Object cleaner = getCleanerMethod.invoke(map, new Object[0]);
        //        Method cleanMethod = cleaner.getClass().getMethod("clean");
        //        if(cleanMethod == null) {
        //          return null;
        //        }
        //        cleanMethod.invoke(cleaner);
        //      }
        //      catch(Exception e) {
        //        LoggingUtil.exception(e);
        //      }
        //      return null;
        //    }
        //  });
        //}
    }
}
