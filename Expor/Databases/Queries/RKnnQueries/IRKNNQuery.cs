using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RKnnQueries
{

    public interface IRKNNQuery : IDatabaseQuery
    {
        /**
         * Get the reverse k nearest neighbors for a particular id.
         * 
         * @param id query object ID
         * @param k number of neighbors requested
         * @return reverse k nearest neighbors
         */
        IList<IDistanceResultPair> GetRKNNForDbId(IDbIdRef id, int k);

        /**
         * Get the reverse k nearest neighbors for a particular object.
         * 
         * @param obj query object instance
         * @param k number of neighbors requested
         * @return reverse k nearest neighbors
         */
        IList<IDistanceResultPair> GetRKNNForObject(IDataVector obj, int k);

        /**
         * Bulk query method for reverse k nearest neighbors for ids.
         * 
         * @param ids query object IDs
         * @param k number of neighbors requested
         * @return reverse k nearest neighbors
         */
        IList<IList<IDistanceResultPair>> GetRKNNForBulkDbIds(IArrayDbIds ids, int k);
    }
}
