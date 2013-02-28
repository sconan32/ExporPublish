using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{
    /**
     * Abstract base class for the page file API for both caches and true page files
     * (in-memory and on-disk).
     * 
     * @author Erich Schubert
     * 
     * @param <P> page type
     */
    public abstract class AbstractPageFile<P> : IPageFile<P>
        where P : IPage
    {
        /**
         * The read I/O-Access of this file.
         */
        protected long readAccess;

        /**
         * The write I/O-Access of this file.
         */
        protected long writeAccess;

        /**
         * Constructor.
         */
        public AbstractPageFile() :
            base()
        {
            this.readAccess = 0;
            this.writeAccess = 0;
        }

        /**
         * Writes a page into this file. The method tests if the page has already an
         * id, otherwise a new id is assigned and returned.
         * 
         * @param page the page to be written
         * @return the id of the page
         */

        public int WritePage(P page)
        {
            lock (this)
            {
                int pageid = SetPageID(page);
                WritePage(pageid, page);
                return pageid;
            }
        }

        /**
         * Perform the actual page write operation.
         * 
         * @param pageid Page id
         * @param page Page to write
         */
        public  abstract void WritePage(int pageid, P page);


        public virtual void Close()
        {
            Clear();
        }


        public long GetReadOperations()
        {
            return readAccess;
        }


        public long GetWriteOperations()
        {
            return writeAccess;
        }


        public void ResetPageAccess()
        {
            this.readAccess = 0;
            this.writeAccess = 0;
        }

        public abstract int SetPageID(P page);

        public abstract P ReadPage(int pageID);

        public abstract void DeletePage(int pageID);


        public abstract void Clear();


        public abstract int GetNextPageID();


        public abstract void SetNextPageID(int nextPageID);


        public abstract int GetPageSize();


        public abstract bool Initialize(IPageHeader header);


        public abstract IPageFileStatistics GetInnerStatistics();

    }
}
