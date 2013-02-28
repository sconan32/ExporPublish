using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{
    public interface IPageHeader
    {
        /**
         * Returns the size of this header in Bytes.
         * 
         * @return the size of this header in Bytes
         */
        int Count { get; }

        /**
         * Initializes this header from the specified file.
         * 
         * @param file the file to which this header belongs
         * @throws IOException if an I/O-error occurs during reading
         */
        void ReadHeader(FileStream file);

        /**
         * Initializes this header from the specified file.
         * 
         * @param data byte array with the page data.
         */
        void ReadHeader(byte[] data);

        /**
         * Writes this header to the specified file.
         * 
         * @param file the file to which this header belongs
         * @throws IOException IOException if an I/O-error occurs during writing
         */
        void WriteHeader(FileStream file);

        /**
         * Return the header as byte array
         * 
         * @return header as byte array
         */
        byte[] AsByteArray();

        /**
         * Returns the size of a page in Bytes.
         * 
         * @return the size of a page in Bytes
         */
        int GetPageSize();

        /**
         * Returns the number of pages necessary for the header
         * 
         * @return the number of pages
         */
        int GetReservedPages();
    }

}
