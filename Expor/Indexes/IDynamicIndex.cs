using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Indexes
{

    /**
     * Index that supports dynamic insertions and removals.
     * 
     * @author Erich Schubert
     *
     * @apiviz.excludeSubtypes
     */
    public interface IDynamicIndex : IIndex
    {
        /**
         * Deletes the specified object from this index.
         * 
         * @param id Object to remove
         * @return true if this index did contain the object, false otherwise
         */
        bool Delete(IDbIdRef id);

        /**
         * Inserts the specified object into this index.
         * 
         * @param id the object to be inserted
         */
        void Insert(IDbIdRef id);

        ///**
        // * Deletes the specified objects from this index.
        // * 
        // * @param ids Objects to remove
        // */
        //void DeleteAll(IDbIds ids);

        ///**
        // * Inserts the specified objects into this index. If a bulk load mode is
        // * implemented, the objects are inserted in one bulk.
        // * 
        // * @param ids the objects to be inserted
        // */
        //void InsertAll(IDbIds ids);
    }

}
