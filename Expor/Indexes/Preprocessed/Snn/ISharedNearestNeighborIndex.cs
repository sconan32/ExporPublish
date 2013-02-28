using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Indexes.Preprocessed.Snn
{

    public interface ISharedNearestNeighborIndex<O> : IIndex
    {
        /**
         * Get the precomputed nearest neighbors
         * 
         * @param id Object ID
         * @return Neighbor IDbIds
         */
        IArrayDbIds GetNearestNeighborSet(IDbIdRef id);

        /**
         * Get the number of neighbors
         * 
         * @return NN size
         */
        int GetNumberOfNeighbors();

        /**
         * Factory interface
         * 
         * @author Erich Schubert
         * 
         * @apiviz.stereotype factory
         * @apiviz.uses SharedNearestNeighborIndex oneway - - 芦create禄
         * 
         * @param <O> The input object type
         * @param <I> Index type produced
         */
    }
    public interface ISharedNearestNeighborIndexFactory<O> : IIndexFactory
    {
        /**
         * Instantiate the index for a given database.
         * 
         * @param database Database type
         * 
         * @return Index
         */

       // IIndex Instantiate(IRelation database);

        /**
         * Get the number of neighbors
         * 
         * @return NN size
         */
        int GetNumberOfNeighbors();
    }
}

