using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Databases.Ids
{

    /**
     * Interface for a collection of database references (IDs).
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     * @apiviz.composedOf DBID
     * @apiviz.has DBIDIter
     */
    public interface IDbIds : IEnumerable<IDbId>
    {


        /**
         * Retrieve the collection / data size.
         * 
         * @return collection size
         */
        public int Count { get; }

        /**
         * Test whether an ID is contained.
         * 
         * @param o object to test
         * @return true when contained
         */
        public bool Contains(IDbIdRef o);

        /**
         * Test for an empty DBID collection.
         * 
         * @return true when empty.
         */
        public bool IsEmpty();

        /**
         * Classic iterator.
         * 
         * @deprecated Use {@link DBIDIter} API instead.
         */

        /**
      * Get a DBID iterator (a more efficient API).
      * 
      * usage example:
      * 
      * <pre>
      * {@code
      * for(DBIDIter iter = ids.iter(); iter.valid(); iter.advance()) {
      *   DBID id = iter.getDBID();
      * }
      * }
      * </pre>
      * 
      * @return iterator
      */

        public override IEnumerator<IDbId> GetEnumerator();
    }

}
