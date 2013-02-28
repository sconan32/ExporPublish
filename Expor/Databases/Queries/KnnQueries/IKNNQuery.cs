using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.DataStructures.Heap;

namespace Socona.Expor.Databases.Queries.KnnQueries
{

    public interface IKNNQuery : IDatabaseQuery
    {
        /**
         * Get the k nearest neighbors for a particular id.
         * 
         * @param id query object ID
         * @param k Number of neighbors requested
         * @return neighbors
         */
        IKNNList GetKNNForDbId(IDbIdRef id, int k);

        /**
         * Bulk query method
         * 
         * @param ids query object IDs
         * @param k Number of neighbors requested
         * @return neighbors
         */
        IList<IKNNList> GetKNNForBulkDbIds(IArrayDbIds ids, int k);

        /**
         * Bulk query method configured by a map.
         * 
         * Warning: this API is not optimal, and might be removed soon (in fact, it is
         * used in a single place)
         * 
         * @param heaps Map of heaps to fill.
         */
        void GetKNNForBulkHeaps(IDictionary<IDbId, IKNNHeap> heaps);

        /**
         * Get the k nearest neighbors for a particular id.
         * 
         * @param obj Query object
         * @param k Number of neighbors requested
         * @return neighbors
         */
        IKNNList GetKNNForObject(IDataVector obj, int k);
    }
}
