using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Resluts;
using Socona.Clustering.Databases.Queries;
using Socona.Clustering.Databases.Ids;
using Socona.Clustering.Data.Types;

namespace Socona.Clustering.Databases.Relations
{

    /**
     * An object representation from a database
     * 
     * @author Erich Schubert
     * 
     * @apiviz.uses DBIDRef
     * 
     * @param <O> Object type
     */
    public interface IRelation<O> : IDatabaseQuery, IHierarchicalResult, IEnumerator<IDbId>
    {
        /**
         * Get the associated database.
         * 
         * Note: in some situations, this might be {@code null}!
         * 
         * @return Database
         */
        public IDatabase getDatabase();

        /**
         * Get the representation of an object.
         * 
         * @param id Object ID
         * @return object instance
         */
        public O Get(IDbIdRef id);

        /**
         * Set an object representation.
         * 
         * @param id Object ID
         * @param val Value
         */
        // TODO: remove / move to a writable API?
        public void Set(IDbIdRef id, O val);

        /**
         * Delete an objects values.
         * 
         * @param id ID to delete
         */
        public void Delete(IDbIdRef id);

        /**
         * Get the data type of this representation
         * 
         * @return Data type
         */
        public SimpleTypeInformation<O> GetDataTypeInformation();

        /**
         * Get the IDs the query is defined for.
         * 
         * @return IDs this is defined for
         */
        public IDbIds GetDBIDs();

        /**
         * Get an iterator access to the DBIDs.
         * 
         * To iterate over all IDs, use the following code fragment:
         * 
         * <pre>
         * {@code
         * for(DBIDIter iter = relation.iterDBIDs(); iter.valid(); iter.advance()) {
         *    DBID id = iter.getDBID();
         * }
         * }
         * </pre>
         * 
         * @return iterator for the DBIDs.
         */

        public override IEnumerator<IDbId> GetEnumerator();
        /**
         * Get the number of DBIDs.
         * 
         * @return Size
         */
        public int Count { get; }
    }
}
