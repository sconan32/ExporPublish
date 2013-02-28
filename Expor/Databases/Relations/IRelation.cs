using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results;
using Socona.Expor.Databases.Queries;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Data.Types;
using Socona.Expor.Data;

namespace Socona.Expor.Databases.Relations
{

    public interface IRelation : IDatabaseQuery, IHierarchicalResult, IEnumerable<IDbId>
    {
        /**
         * Get the associated database.
         * 
         * Note: in some situations, this might be {@code null}!
         * 
         * @return Database
         */
        IDatabase GetDatabase();



        object this[IDbIdRef id]
        { get; set; }
        /**
         * Get the representation of an object.
         * 
         * @param id Object ID
         * @return object instance
         */
        //object GetRaw(IDbIdRef id);

        ///**
        // * Set an object representation.
        // * 
        // * @param id Object ID
        // * @param val Value
        // */
        //// TODO: remove / move to a writable API?
        //// void Set(IDbIdRef id, T val);
        //void Set(IDbIdRef id, object val);
        IDataVector DataAt(IDbIdRef id);
        INumberVector VectorAt(IDbIdRef id);

        /**
         * Delete an objects values.
         * 
         * @param id ID to delete
         */
        void Delete(IDbIdRef id);

        /**
         * Get the data type of this representation
         * 
         * @return Data type
         */
        SimpleTypeInformation GetDataTypeInformation();

        /**
         * Get the IDs the query is defined for.
         * 
         * @return IDs this is defined for
         */
        IDbIds GetDbIds();

        /**
         * Get an iterator access to the DbIds.
         * 
         * To iterate over all IDs, use the following code fragment:
         * 
         * @return iterator for the DbIds.
         */
        // IEnumerator<IDbId> GetEnumerator();
        /// <summary>
        ///   Get the number of DbIds.
        /// </summary>

        int Count { get; }
    }
}
