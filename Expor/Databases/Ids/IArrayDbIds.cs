using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{
    /**
  * Interface for array based DBIDs.
  * 
  * @author Erich Schubert
  */
    public interface IArrayDbIds : IDbIds
    {
        /**
         * Get the i'th entry (starting at 0)
         * 
         * @param i Index
         * @return DBID of i'th entry.
         */
        IDbId this[int index]
        {
            get;
            set;
        }





        /**
         * Search for the position of the given key, assuming that the data set is
         * sorted.
         * 
         * For keys not found, <code>-(1+insertion position)</code> is returned, as
         * for Java {@link java.util.Collections#binarySearch}
         * 
         * @param key Key to search for
         * @return Offset of key
         */
        int BinarySearch(IDbIdRef key);
        /**
         * Slice a subarray (as view, not copy!)
         * 
         * @param begin Begin (inclusive)
         * @param end End (exclusive)
         * @return Array slice.
         */
        IArrayDbIds Slice(int begin, int end);
    }

}
