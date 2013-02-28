using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    /**
     * A memory based implementation of a PageFile that simulates I/O-access.<br>
     * Implemented as a Map with keys representing the ids of the saved pages.
     * 
     * @author Elke Achtert
     * 
     * @param <P> Page type
     */
    public class MemoryPageFile<P> : AbstractStoringPageFile<P>
        where P : IPage
    {
        /**
         * Holds the pages.
         */
        private Dictionary<int, P> file;

        /**
         * Creates a new MemoryPageFile that is supported by a cache with the
         * specified parameters.
         * 
         * @param pageSize the size of a page in Bytes
         */
        public MemoryPageFile(int pageSize) :
            base(pageSize)
        {
            this.file = new Dictionary<int, P>();
        }


        public override P ReadPage(int pageID)
        {
            lock (this)
            {
                readAccess++;
                return file[(pageID)];
            }
        }


        public  override void WritePage(int pageID, P page)
        {
            writeAccess++;
            file[pageID] = page;
            page.SetDirty(false);
        }


        public override void DeletePage(int pageID)
        {
            lock (this)
            {
                // put id to empty nodes and
                // delete from cache
                base.DeletePage(pageID);

                // delete from file
                writeAccess++;
                file.Remove(pageID);
            }
        }


        public override void Clear()
        {

            file.Clear();
        }
    }
}
