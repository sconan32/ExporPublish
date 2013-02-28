using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Persistent;
using Socona.Expor.Results;

namespace Socona.Expor.Indexes
{

    public interface IIndex : IResult
    {
        /**
         * Get the underlying page file (or a proxy), for access counts.
         * 
         * @return page file
         */
        IPageFileStatistics GetPageFileStatistics();

        /**
         * Inserts the specified object into this index.
         * 
         * @param id the object to be inserted
         */
        void Insert(IDbId id);

        /**
         * Inserts the specified objects into this index. If a bulk load mode is
         * implemented, the objects are inserted in one bulk.
         * 
         * @param ids the objects to be inserted
         */
        void InsertAll(IDbIds ids);

        /**
         * Deletes the specified object from this index.
         * 
         * @param id Object to remove
         * @return true if this index did contain the object, false otherwise
         */
        bool Delete(IDbId id);

        /**
         * Deletes the specified objects from this index.
         * 
         * @param ids Objects to remove
         */
        void DeleteAll(IDbIds ids);
    }
}
