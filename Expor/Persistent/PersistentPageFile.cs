using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Socona.Expor.Indexes.Tree;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.Persistent
{

    /**
     * A PersistentPageFile stores objects persistently that implement the
     * <code>Page</code> interface. For convenience each page is represented by a
     * single file. All pages are stored in a specified directory.
     * 
     * @author Elke Achtert
     * 
     * @apiviz.composedOf PageHeader
     * @apiviz.composedOf RandomAccessFile
     * 
     * @param <P> Page type
     */
    public class PersistentPageFile<P> : AbstractStoringPageFile<P>
    where P : IExternalizablePage
    {
        /**
         * Our logger
         */
        private static Logging logger = Logging.GetLogger(typeof(PersistentPageFile<P>));

        /**
         * Indicates an empty page.
         */
        private static int EMPTY_PAGE = 0;

        /**
         * Indicates a filled page.
         */
        private static int FILLED_PAGE = 1;

        /**
         * The file storing the pages.
         */
        private FileStream file;

        /**
         * The header of this page file.
         */
        protected IPageHeader header;

        /**
         * The type of pages we use.
         */
        protected Type pageclass;

        /**
         * Whether we are initializing from an existing file.
         */
        private bool existed;


        protected BinaryFormatter formatter;
        /**
         * Creates a new PersistentPageFile from an existing file.
         * 
         * @param pageSize the page size
         * @param pageclass the class of pages to be used
         */
        public PersistentPageFile(int pageSize, String fileName, Type pageclass) :
            base(pageSize)
        {
            this.formatter = new BinaryFormatter();
            this.pageclass = pageclass;
            // init the file


            // create from existing file
            existed = File.Exists(fileName);
            try
            {
                file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (IOException e)
            {
                throw new AbortException("IO error in loading persistent page file.", e);
            }
        }

        /**
         * Reads the page with the given id from this file.
         * 
         * @param pageID the id of the page to be returned
         * @return the page with the given pageId
         */

        public override P ReadPage(int pageID)
        {
            try
            {
                readAccess++;
                long offset = ((long)(header.GetReservedPages() + pageID)) * (long)pageSize;
                byte[] buffer = new byte[pageSize];
                file.Seek(offset, SeekOrigin.Begin);
                file.Read(buffer, 0, pageSize);
                return ByteArrayToPage(buffer);
            }
            catch (IOException e)
            {
                throw new IOException("IOException occurred during reading of page " + pageID + "\n", e);
            }
        }

        /**
         * Deletes the node with the specified id from this file.
         * 
         * @param pageID the id of the node to be deleted
         */

        public override void DeletePage(int pageID)
        {
            try
            {
                // / put id to empty pages list
                base.DeletePage(pageID);

                // delete from file
                writeAccess++;
                byte[] array = PageToByteArray(default(P));
                long offset = ((long)(header.GetReservedPages() + pageID)) * (long)pageSize;
                file.Seek(offset, SeekOrigin.Begin);
                file.Write(array, 0, array.Length);
            }
            catch (IOException )
            {
                throw;
            }
        }

        /**
         * This method is called by the cache if the <code>page</code> is not longer
         * stored in the cache and has to be written to disk.
         * 
         * @param page the page which has to be written to disk
         */

        public override void WritePage(int pageID, P page)
        {
            try
            {
                writeAccess++;
                byte[] array = PageToByteArray(page);
                long offset = ((long)(header.GetReservedPages() + pageID)) * (long)pageSize;
                Debug.Assert(offset >= 0, header.GetReservedPages() + " " + pageID + " " + pageSize + " " + offset);
                file.Seek(offset, SeekOrigin.Begin);
                file.Write(array, 0, array.Length);
                page.SetDirty(false);
            }
            catch (IOException e)
            {
                throw new IOException("Error writing to page file.", e);
            }
        }

        /**
         * Closes this file.
         */

        public override void Close()
        {
            try
            {
                base.Close();
                if (emptyPages.Count > 0 && header is TreeIndexHeader)
                {
                    // write the list of empty pages to the end of the file
                    ((TreeIndexHeader)header).WriteEmptyPages(emptyPages, file);
                }
                ((TreeIndexHeader)header).SetLargestPageID(nextPageID);
                header.WriteHeader(file);
                file.Close();
            }
            catch (IOException)
            {
                throw;
            }
        }

        /**
         * Clears this PageFile.
         */

        public override void Clear()
        {
            try
            {
                file.SetLength(header.Count);
            }
            catch (IOException)
            {
                throw;
            }
        }

        /**
         * Reconstruct a serialized object from the specified byte array.
         * 
         * @param array the byte array from which the object should be reconstructed
         * @return a serialized object from the specified byte array
         */
        private P ByteArrayToPage(byte[] array)
        {
            try
            {
                MemoryStream bais = new MemoryStream();
                bais.Write(array, 0, array.Length);

                int type = (Int32)formatter.Deserialize(bais);
                if (type == EMPTY_PAGE)
                {
                    return default(P);
                }
                else if (type == FILLED_PAGE)
                {
                    P page;
                    try
                    {

                        page = (P)formatter.Deserialize(bais);
                    }
                    catch (Exception e)
                    {
                        throw new AbortException("Error instanciating an index page", e);
                    }
                    return page;
                }
                else
                {
                    throw new ArgumentException("Unknown type: " + type);
                }
            }
            catch (IOException e)
            {
                throw new AbortException("IO Error in page file", e);
            }
        }

        /**
         * Serializes an object into a byte array.
         * 
         * @param page the object to be serialized
         * @return the byte array
         */
        private byte[] PageToByteArray(P page)
        {
            try
            {
                if (page == null)
                {
                    MemoryStream baos = new MemoryStream();
                    formatter.Serialize(baos, EMPTY_PAGE);
                    baos.Seek(0, SeekOrigin.Begin);
                    byte[] array = new byte[pageSize];
                    baos.Read(array, 0, (int)pageSize);
                    baos.Close();
                    return array;
                }
                else
                {
                    MemoryStream baos = new MemoryStream();

                    formatter.Serialize(baos, FILLED_PAGE);
                    formatter.Serialize(baos, page);

                    baos.Seek(0, SeekOrigin.Begin);
                    byte[] array = new byte[baos.Length];
                    baos.Read(array, 0, (int)baos.Length);
                    baos.Close();
                    if (array.Length > this.pageSize)
                    {
                        throw new ArgumentException("Size of page " + page +
                            " is greater than specified" + " pagesize: " + array.Length + " > " + pageSize);
                    }
                    else if (array.Length == this.pageSize)
                    {
                        return array;
                    }

                    else
                    {
                        byte[] result = new byte[pageSize];
                        Array.Copy(array, 0, result, 0, array.Length);
                        return result;
                    }
                }
            }
            catch (IOException)
            {
                throw;
            }
        }

        /** @return the random access file storing the pages. */
        public FileStream GetFile()
        {
            return file;
        }

        /**
         * Get the header of this persistent page file.
         * 
         * @return the header used by this page file
         */
        public IPageHeader GetHeader()
        {
            return header;
        }

        /**
         * Increases the {@link AbstractStoringPageFile#readAccess readAccess} counter by
         * one.
         */
        public void IncreaseReadAccess()
        {
            readAccess++;
        }

        /**
         * Increases the {@link AbstractStoringPageFile#writeAccess writeAccess} counter by
         * one.
         */
        public void IncreaseWriteAccess()
        {
            writeAccess++;
        }

        /**
         * Set the next page id to the given value. If this means that any page ids
         * stored in <code>emptyPages</code> are smaller than
         * <code>next_page_id</code>, they are removed from this file's observation
         * stack.
         * 
         * @param next_page_id the id of the next page to be inserted (if there are no
         *        more empty pages to be filled)
         */

        public override void SetNextPageID(int next_page_id)
        {
            this.nextPageID = next_page_id;
            while (emptyPages.Count > 0 && emptyPages.Peek() >= this.nextPageID)
            {
                emptyPages.Pop();
            }
        }


        public override bool Initialize(IPageHeader header)
        {
            try
            {
                if (existed)
                {
                    logger.Debug("Initializing from an existing page file.");

                    // init the header
                    this.header = header;
                    header.ReadHeader(file);

                    // reading empty nodes in Stack
                    if (header is TreeIndexHeader)
                    {
                        TreeIndexHeader tiHeader = (TreeIndexHeader)header;
                        nextPageID = tiHeader.GetLargestPageID();
                        try
                        {
                            emptyPages = tiHeader.ReadEmptyPages(file);
                        }
                        catch (Exception )
                        {
                            throw;
                        }
                    }
                    else
                    { // must scan complete file
                        int i = 0;
                        while (file.Position + pageSize <= file.Length)
                        {
                            long offset = ((long)(header.GetReservedPages() + i)) * (long)pageSize;
                            byte[] buffer = new byte[pageSize];
                            file.Seek(offset, SeekOrigin.Begin);
                            file.Read(buffer, 0, pageSize);

                            MemoryStream bais = new MemoryStream(buffer);

                            int type = (int)formatter.Deserialize(bais);
                            if (type == EMPTY_PAGE)
                            {
                                emptyPages.Push(i);
                            }
                            else if (type == FILLED_PAGE)
                            {
                                nextPageID = i + 1;
                            }
                            else
                            {
                                throw new ArgumentException("Unknown type: " + type);
                            }
                            i++;
                        }
                    }
                }
                // create new file
                else
                {
                    logger.Debug("Initializing with a new page file.");

                    // writing header
                    this.header = header;
                    header.WriteHeader(file);
                }
            }
            catch (IOException )
            {
                throw;
            }

            // Return "new file" status
            return existed;
        }


    }
}
