using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RangeQueries
{

    public interface IRangeQuery : IDatabaseQuery
    {
        /**
         * Get the nearest neighbors for a particular id in a given query range
         * 
         * @param id query object ID
         * @param range Query range
         * @return neighbors
         */
        IDistanceDbIdList GetRangeForDbId(IDbIdRef id, IDistanceValue range);

        /**
         * Get the nearest neighbors for a particular object in a given query range
         * 
         * @param obj Query object
         * @param range Query range
         * @return neighbors
         */
        IDistanceDbIdList GetRangeForObject(IDataVector obj, IDistanceValue range);
    }
}
