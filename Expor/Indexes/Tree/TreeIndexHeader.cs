using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Socona.Expor.Persistent;

namespace Socona.Expor.Indexes.Tree
{

    public class TreeIndexHeader : DefaultPageHeader
    {
        /**
         * The size of this header in Bytes, which is 20 Bytes ( 4 Bytes for
         * {@link #dirCapacity}, {@link #leafCapacity}, {@link #dirMinimum},
         * {@link #leafMinimum}, and {@link #emptyPagesSize}).
         */
        private static int SIZE = 20;

        /**
         * The capacity of a directory node (= 1 + maximum number of entries in a
         * directory node).
         */
        int dirCapacity;

        /**
         * The capacity of a leaf node (= 1 + maximum number of entries in a leaf
         * node).
         */
        int leafCapacity;

        /**
         * The minimum number of entries in a directory node.
         */
        int dirMinimum;

        /**
         * The minimum number of entries in a leaf node.
         */
        int leafMinimum;

        /**
         * The number of bytes Additionally needed for the listing of empty pages of
         * the headed page file.
         */
        private int emptyPagesSize = 0;
        /**
         * The largest ID used so far
         */
        private int largestPageID = 0;
        /**
         * Empty constructor for serialization.
         */
        public TreeIndexHeader() :
            base()
        {
        }

        /**
         * Creates a new header with the specified parameters.
         * 
         * @param pageSize the size of a page in bytes
         * @param dirCapacity the maximum number of entries in a directory node
         * @param leafCapacity the maximum number of entries in a leaf node
         * @param dirMinimum the minimum number of entries in a directory node
         * @param leafMinimum the minimum number of entries in a leaf node
         */
        public TreeIndexHeader(int pageSize, int dirCapacity, int leafCapacity, int dirMinimum, int leafMinimum) :
            base(pageSize)
        {
            this.dirCapacity = dirCapacity;
            this.leafCapacity = leafCapacity;
            this.dirMinimum = dirMinimum;
            this.leafMinimum = leafMinimum;
        }

        /**
         * Initializes this header from the specified file. Calls
         * {@link de.lmu.ifi.dbs.elki.persistent.DefaultPageHeader#readHeader(java.io.RandomAccessFile)
         * DefaultPageHeader#readHeader(file)} and reads the integer values of
         * {@link #dirCapacity}, {@link #leafCapacity}, {@link #dirMinimum},
         * {@link #leafMinimum} and {@link #emptyPagesSize} from the file.
         */

        public override void ReadHeader(FileStream file)
        {
            base.ReadHeader(file);

            this.dirCapacity = base.ReadInt(file);
            this.leafCapacity = base.ReadInt(file);
            this.dirMinimum = base.ReadInt(file);
            this.leafMinimum = base.ReadInt(file);
            this.emptyPagesSize = base.ReadInt(file);
            this.largestPageID = base.ReadInt(file);
        }

        /**
         * Writes this header to the specified file. Writes the integer values of
         * {@link #dirCapacity}, {@link #leafCapacity}, {@link #dirMinimum},
         * {@link #leafMinimum} and {@link #emptyPagesSize} to the file.
         */

        public override void WriteHeader(FileStream file)
        {
            base.WriteHeader(file);
            base.WriteInt(file, this.dirCapacity);
            base.WriteInt(file, this.leafCapacity);
            base.WriteInt(file, this.dirMinimum);
            base.WriteInt(file, this.leafMinimum);
            base.WriteInt(file, this.emptyPagesSize);
            base.WriteInt(file, this.largestPageID);
        }

        /**
         * Returns the capacity of a directory node (= 1 + maximum number of entries
         * in a directory node).
         * 
         * @return the capacity of a directory node (= 1 + maximum number of entries
         *         in a directory node)
         */
        public int GetDirCapacity()
        {
            return dirCapacity;
        }

        /**
         * Returns the capacity of a leaf node (= 1 + maximum number of entries in a
         * leaf node).
         * 
         * @return the capacity of a leaf node (= 1 + maximum number of entries in a
         *         leaf node)
         */
        public int GetLeafCapacity()
        {
            return leafCapacity;
        }

        /**
         * Returns the minimum number of entries in a directory node.
         * 
         * @return the minimum number of entries in a directory node
         */
        public int GetDirMinimum()
        {
            return dirMinimum;
        }

        /**
         * Returns the minimum number of entries in a leaf node.
         * 
         * @return the minimum number of entries in a leaf node
         */
        public int GetLeafMinimum()
        {
            return leafMinimum;
        }

        /** @return the number of bytes needed for the listing of empty pages */
        public int GetEmptyPagesSize()
        {
            return emptyPagesSize;
        }

        /**
         * Set the size required by the listing of empty pages.
         * 
         * @param emptyPagesSize the number of bytes needed for this listing of empty
         *        pages
         */
        public void SetEmptyPagesSize(int emptyPagesSize)
        {
            this.emptyPagesSize = emptyPagesSize;
        }


        public int GetLargestPageID()
        {
            return largestPageID;
        }

        public void SetLargestPageID(int largestPageID)
        {
            this.largestPageID = largestPageID;
        }

        /**
         * Returns {@link de.lmu.ifi.dbs.elki.persistent.DefaultPageHeader#size()}
         * plus the value of {@link #SIZE}). Note, this is only the base size and
         * probably <em>not</em> the overall size of this header, as there may be
         * empty pages to be maintained.
         */

        public override int Count
        {
            get { return base.Count + SIZE; }
        }

        /**
         * Write the indices of empty pages the the end of <code>file</code>. Calling
         * this method should be followed by a {@link #writeHeader(RandomAccessFile)}.
         * 
         * @param emptyPages the stack of empty page ids which remain to be filled
         * @param file File to work with
         * @throws IOException thrown on IO errors
         */
        public void WriteEmptyPages(Stack<Int32> emptyPages, FileStream file)
        {
            if (emptyPages.Count == 0)
            {
                this.emptyPagesSize = 0;
                return; // nothing to write
            }
            BinaryFormatter bf = new BinaryFormatter();
            file.Seek(0, SeekOrigin.End);
            long oldlen = file.Length;

            bf.Serialize(file, emptyPages);
            this.emptyPagesSize = (int)(file.Length - oldlen);

            //ByteArrayOutputStream baos = new ByteArrayOutputStream();
            //ObjectOutputStream oos = new ObjectOutputStream(baos);
            //oos.writeObject(emptyPages);
            //oos.flush();
            //byte[] bytes = baos.toByteArray();
            //this.emptyPagesSize = bytes.Length;
            //oos.close();
            //baos.close();
            //if(this.emptyPagesSize > 0) {
            //  file.Seek(0, SeekOrigin.End);
            //  file.Write(bytes);
            //}
        }

        /**
         * Read the empty pages from the end of <code>file</code>.
         * 
         * @param file File to work with
         * @return a stack of empty pages in <code>file</code>
         * @throws IOException thrown on IO errors
         * @throws ClassNotFoundException if the stack of empty pages could not be
         *         correctly read from file
         */

        public Stack<int> ReadEmptyPages(FileStream file)
        {
            if (emptyPagesSize == 0)
            {
                return new Stack<int>();
            }
            BinaryFormatter bf = new BinaryFormatter();
            file.Seek(emptyPagesSize - file.Length, SeekOrigin.End);

            Stack<int> emptyPages = (Stack<int>)bf.Deserialize(file);

            return emptyPages;
        }

    }

}
