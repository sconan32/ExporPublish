using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    /**
     * Abstract class implementing general methods of a PageFile. A PageFile stores
     * objects that implement the <code>Page</code> interface.
     * 
     * @author Elke Achtert
     * 
     * @apiviz.has Page
     * @apiviz.has PageFileStatistics
     * 
     * @param <P> Page type
     */
    public abstract class AbstractStoringPageFile<P> : AbstractPageFile<P>
    where P : IPage
    {
        /**
         * A stack holding the empty page ids.
         */
        protected Stack<int> emptyPages;

        /**
         * The last page ID.
         */
        protected int nextPageID;

        /**
         * The size of a page in Bytes.
         */
        protected int pageSize;

        /**
         * Creates a new PageFile.
         */
        protected AbstractStoringPageFile(int pageSize)
        {
            this.emptyPages = new Stack<int>();
            this.nextPageID = 0;
            this.pageSize = pageSize;
        }

        /**
         * Sets the id of the given page.
         * 
         * @param page the page to set the id
       */
        public override int SetPageID(P page)
        {
            int pageID = page.GetPageID();
            if (pageID == -1)
            {
                pageID = GetNextEmptyPageID();
                if (pageID == -1)
                {
                    pageID = nextPageID++;
                }
                page.SetPageID(pageID);
            }
            return pageID;
        }

        /**
         * Deletes the node with the specified id from this file.
         * 
         * @param pageID the id of the node to be deleted
         */

        public override void DeletePage(int pageID)
        {
            // put id to empty nodes
            emptyPages.Push(pageID);
        }

        /**
         * Returns the next empty page id.
         * 
         * @return the next empty page id
         */
        private  int GetNextEmptyPageID()
        {
            if (emptyPages.Count > 0)
            {
                return emptyPages.Pop();
            }
            else
            {
                return -1;
            }
        }

        /**
         * Returns the next page id.
         * 
         * @return the next page id
         */

        public override int GetNextPageID()
        {
            return nextPageID;
        }

        /**
         * Sets the next page id.
         * 
         * @param nextPageID the next page id to be set
         */

        public override void SetNextPageID(int nextPageID)
        {
            this.nextPageID = nextPageID;
        }

        /**
         * Get the page size of this page file.
         * 
         * @return page size
         */

        public override int GetPageSize()
        {
            return pageSize;
        }

        /**
         * Initialize the page file with the given header - return "true" if the file
         * already existed.
         * 
         * @param header Header
         * @return true when the file already existed.
         */

        public override bool Initialize(IPageHeader header)
        {
            this.pageSize = header.GetPageSize();
            return false;
        }


        public override IPageFileStatistics GetInnerStatistics()
        {
            // Default: no nested page file.
            return null;
        }

    }
}
