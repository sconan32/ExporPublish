using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    public interface IPageFile<P> : IPageFileStatistics where P : IPage
    {
        /**
         * Sets the id of the given page.
         * 
         * @param page the page to set the id
         * @return the page id
         */
        int SetPageID(P page);

        /**
         * Writes a page into this file. The method tests if the page has already an
         * id, otherwise a new id is assigned and returned.
         * 
         * @param page the page to be written
         * @return the id of the page
         */
        int WritePage(P page);

        /**
         * Reads the page with the given id from this file.
         * 
         * @param pageID the id of the page to be returned
         * @return the page with the given pageId
         */
        P ReadPage(int pageID);

        /**
         * Deletes the node with the specified id from this file.
         * 
         * @param pageID the id of the node to be deleted
         */
        void DeletePage(int pageID);

        /**
         * Closes this file.
         */
        void Close();

        /**
         * Clears this PageFile.
         */
        void Clear();

        /**
         * Returns the next page id.
         * 
         * @return the next page id
         */
        int GetNextPageID();

        /**
         * Sets the next page id.
         * 
         * @param nextPageID the next page id to be set
         */
        void SetNextPageID(int nextPageID);

        /**
         * Get the page size of this page file.
         * 
         * @return page size
         */
        int GetPageSize();

        /**
         * Initialize the page file with the given header - return "true" if the file
         * already existed.
         * 
         * @param header Header
         * @return true when the file already existed.
         */
        bool Initialize(IPageHeader header);
    }
}
