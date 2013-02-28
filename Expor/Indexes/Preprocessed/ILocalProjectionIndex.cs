using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Maths.LinearAlgebra;

namespace Socona.Expor.Indexes.Preprocessed
{

    public interface ILocalProjectionIndex : IIndex
    {
        /**
         * Get the precomputed local projection for a particular object ID.
         * 
         * @param id Object ID
         * @return local projection
         */
        IProjectionResult GetLocalProjection(IDbIdRef id);

    }

    /**
     * Factory
     * 
     * @author Erich Schubert
     * 
     * @apiviz.stereotype factory
     * @apiviz.has LocalProjectionIndex oneway - - 芦create禄
     * 
     * @param <V> List type
     * @param <I> Index type
     */
    public interface ILocalProjectionIndexFactory: IIndexFactory
    {
        /**
         * Instantiate the index for a given database.
         * 
         * @param relation Relation to use
         * 
         * @return Index
         */

        // ILocalProjectionIndex<V> Instantiate(IRelation relation);
    }
}
