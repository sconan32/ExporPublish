using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.Persistent
{

    /**
     * An LRU cache, based on <code>LinkedHashMap</code>.<br>
     * This cache has a fixed maximum number of objects (<code>cacheSize</code>). If
     * the cache is full and another object is added, the LRU (least recently used)
     * object is dropped.
     * 
     * @author Elke Achtert
     * 
     * @apiviz.composedOf PageFile
     * 
     * @param <P> Page type
     */
    public class LRUCache<P> : AbstractPageFile<P>
    where P : IPage
    {
        /**
         * Our logger
         */
        private static Logging logger = Logging.GetLogger(typeof(LRUCache<P>));

        /**
         * Cache size in bytes.
         */
        protected long cacheSizeBytes;

        /**
         * The maximum number of objects in this cache.
         */
        protected long cacheSize;

        /**
         * The map holding the objects of this cache.
         */
        private Utilities.DataStructures.Collections.LRUCache<int, P> map;

        /**
         * The underlying file of this cache. If an object is dropped it is written to
         * the file.
         */
        protected IPageFile<P> file;

        /**
         * Initializes this cache with the specified parameters.
         * 
         * @param cacheSizeBytes the maximum number of pages in this cache
         * @param file the underlying file of this cache, if a page is dropped it is
         *        written to the file
         */
        public LRUCache(long cacheSizeBytes, IPageFile<P> file)
        {
            this.file = file;
            this.cacheSizeBytes = cacheSizeBytes;
        }

        /**
         * Retrieves a page from the cache. The retrieved page becomes the MRU (most
         * recently used) page.
         * 
         * @param pageID the id of the page to be returned
         * @return the page associated to the id or null if no value with this key
         *         exists in the cache
         */

        public override P ReadPage(int pageID)
        {
            lock (this)
            {
                readAccess++;

                P page;

                if (map.TryGetValue(pageID, out page))
                {
                    if (logger.IsDebugging)
                    {
                        logger.Debug("Read from cache: " + pageID);
                    }

                }
                else
                {
                    if (logger.IsDebugging)
                    {
                        logger.Debug("Read from backing: " + pageID);
                    }

                    page = file.ReadPage(pageID);
                    map.Add(pageID, page);
                }
                return page;
            }
        }


        public override void WritePage(int pageID, P page)
        {
            lock (this)
            {

                writeAccess++;
                page.SetDirty(true);
                map[pageID] = page;
                if (logger.IsDebugging)
                {
                    logger.Debug("Write to cache: " + pageID);
                }
            }
        }


        public override void DeletePage(int pageID)
        {
            writeAccess++;
            map.Remove(pageID);
            file.DeletePage(pageID);
        }

        /**
         * Write page through to disk.
         * 
         * @param page page
         */
        protected void ExpirePage(P page)
        {
            if (logger.IsDebugging)
            {
                logger.Debug("Write to backing:" + page.GetPageID());
            }
            if (page.IsDirty())
            {
                file.WritePage(page);
            }
        }


        public override int SetPageID(P page)
        {
            int pageID = file.SetPageID(page);
            return pageID;
        }


        public override int GetNextPageID()
        {
            return file.GetNextPageID();
        }


        public override void SetNextPageID(int nextPageID)
        {
            file.SetNextPageID(nextPageID);
        }


        public override int GetPageSize()
        {
            return file.GetPageSize();
        }


        public override bool Initialize(IPageHeader header)
        {
            bool created = file.Initialize(header);
            // Compute the actual cache size.
            this.cacheSize = cacheSizeBytes / header.GetPageSize();

            if (this.cacheSize <= 0)
            {
                throw new AbortException("Invalid cache size: " + cacheSizeBytes + " / " +
                    header.GetPageSize() + " = " + cacheSize);
            }

            if (logger.IsDebugging)
            {
                logger.Debug("LRU cache size is " + cacheSize + " pages.");
            }

            float hashTableLoadFactor = 0.75f;
            int hashTableCapacity = (int)Math.Ceiling(cacheSize / hashTableLoadFactor) + 1;

            this.map = new Utilities.DataStructures.Collections.LRUCache<int, P>();

            return created;
        }


        public override void Close()
        {
            Flush();
            file.Close();
        }

        /**
         * Flushes this caches by writing any entry to the underlying file.
         */
        public void Flush()
        {
            foreach (P obj in map.Values)
            {
                ExpirePage(obj);
            }
            map.Clear();
        }

        /**
         * Returns a string representation of this cache.
         * 
         * @return a string representation of this cache
         */

        public override String ToString()
        {
            return map.ToString();
        }

        /**
         * Clears this cache.
         */
        public override void Clear()
        {
            map.Clear();
        }

        /**
         * Sets the maximum size of this cache.
         * 
         * @param cacheSize the cache size to be set
         */
        public void SetCacheSize(int cacheSize)
        {
            this.cacheSize = cacheSize;

            long toDelete = map.Count - this.cacheSize;
            if (toDelete <= 0)
            {
                return;
            }

            List<int> keys = new List<int>(map.Keys);
            keys.Reverse();

            foreach (Int32 id in keys)
            {
                P page = map[id];
                map.Remove(id);
                file.WritePage(page);
            }
        }


        public override IPageFileStatistics GetInnerStatistics()
        {
            return file;
        }
    }

}
