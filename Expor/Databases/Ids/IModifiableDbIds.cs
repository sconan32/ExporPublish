using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Interface for a generic modifiable DbId collection.
     * 
     * Note: we had this use the Java Collections API for a long time, however this
     * prevented certain optimizations. So it now only mimics the Collections API
     * <em>deliberately</em>.
     * 
     * @author Erich Schubert
     */
    public interface IModifiableDbIds : IDbIds
    {
        /**
         * Add DbIds to collection.
         * 
         * @param ids IDs to add.
         * @return {@code true} when modified
         */
        bool AddDbIds(IDbIds ids);

        /**
         * Remove DbIds from collection.
         * 
         * @param ids IDs to remove.
         * @return {@code true} when modified
         */
        bool RemoveDbIds(IDbIds ids);

        /**
         * Add a single DbId to the collection.
         * 
         * @param id ID to add
         */
        bool Add(IDbIdRef id);

        /**
         * Remove a single DbId from the collection.
         * 
         * @param id ID to remove
         */
        bool Remove(IDbIdRef id);

        /**
         * Clear this collection.
         */
        void Clear();

        /**
         * Get a <em>modifiable</em> DbId iterator (a more efficient API).
         * 
         * usage example:
         * 
         * <pre>
         * {@code
         * for(DbIdMIter iter = ids.iter(); iter.valid(); iter.advance()) {
         *   DbId id = iter.getDbId();
         *   iter.remove();
         * }
         * }
         * </pre>
         * 
         * @return modifiable iterator
         */

        // IEnumerator<IDbId> GetEnumerator();
    }
}
