using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    public class DefaultPageHeader : IPageHeader
    {
        /**
         * The size of this header in Bytes, which is 8 Bytes ( 4 Bytes for
         * {@link #FILE_VERSION} and 4 Bytes for {@link #pageSize}).
         */
        private static int SIZE = 8;

        /**
         * Version number of this header (magic number).
         */
        private static int FILE_VERSION = 841150978;

        /**
         * The size of a page in bytes.
         */
        private int pageSize = -1;

        /**
         * Empty constructor for serialization.
         */
        public DefaultPageHeader()
        {
            // empty constructor
        }

        /**
         * Creates a new header with the specified parameters.
         * 
         * @param pageSize the size of a page in bytes
         */
        public DefaultPageHeader(int pageSize)
        {
            this.pageSize = pageSize;
        }

        /**
         * Returns the value of {@link #SIZE}).
         * 
         */

        public virtual int Count { get { return SIZE; } }

        /**
         * Initializes this header from the specified file. Looks for the right
         * version and reads the integer value of {@link #pageSize} from the file.
         * 
         */

        public virtual void ReadHeader(FileStream file)
        {

            file.Seek(0, SeekOrigin.Begin);
            byte[] buf = new byte[16];
            file.Read(buf, 0, 4);
            int tmp = BitConverter.ToInt32(buf, 0);
            if (tmp != FILE_VERSION)
            {
                throw new ApplicationException("File " + file + " is not a PersistentPageFile or wrong version!");
            }
            file.Read(buf, 0, 4);
            tmp = BitConverter.ToInt32(buf, 0);
            this.pageSize = tmp;
        }

        /**
         * Initializes this header from the given Byte array. Looks for the right
         * version and reads the integer value of {@link #pageSize} from the file.
         * 
         */

        public virtual void ReadHeader(byte[] data)
        {
            if (ByteArrayUtil.ReadInt(data, 0) != FILE_VERSION)
            {
                throw new ApplicationException("PersistentPageFile version does not match!");
            }

            this.pageSize = ByteArrayUtil.ReadInt(data, 4);
        }

        /**
         * Writes this header to the specified file. Writes the {@link #FILE_VERSION
         * version} of this header and the integer value of {@link #pageSize} to the
         * file.
         * 
         */

        public virtual void WriteHeader(FileStream file)
        {
            file.Seek(0, SeekOrigin.Begin);
            byte[] buf = new byte[16];
            buf = BitConverter.GetBytes(FILE_VERSION);
            file.Write(buf, 0, 4);
            buf = BitConverter.GetBytes(this.pageSize);
            file.Write(buf, 0, 4);
        }


        public virtual byte[] AsByteArray()
        {
            byte[] header = new byte[SIZE];
            ByteArrayUtil.WriteInt(header, 0, FILE_VERSION);
            ByteArrayUtil.WriteInt(header, 4, this.pageSize);
            return header;
        }

        protected int ReadInt(FileStream fs)
        {
            byte[] buf = new byte[8];
            fs.Read(buf, 0, 4);
            return BitConverter.ToInt32(buf, 0);
        }
        protected void WriteInt(FileStream fs,int val)
        {
            byte[] buf = new byte[16];
            buf = BitConverter.GetBytes(val);
            fs.Write(buf, 0, 4);
        }
        /**
         * Returns the size of a page in Bytes.
         * 
         * @return the size of a page in Bytes
         */

        public int GetPageSize()
        {
            return pageSize;
        }

        /**
         * Returns the number of pages necessary for the header
         * 
         * @return the number of pages
         */

        public int GetReservedPages()
        {
            return Count / GetPageSize() + 1;
        }
    }

}
