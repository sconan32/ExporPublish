using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RangeQueries
{

    public class LinearScanPrimitiveDistanceRangeQuery<O> : LinearScanRangeQuery<O>
        where O:IDataVector
    {
        /**
         * Constructor.
         * 
         * @param distanceQuery Distance function to use
         */
        public LinearScanPrimitiveDistanceRangeQuery(PrimitiveDistanceQuery<O> distanceQuery) :
            base(distanceQuery)
        {
        }


        public override IDistanceDbIdList GetRangeForDbId(IDbIdRef id, IDistanceValue range)
        {
            // Note: subtle optimization. Get "id" only once!
            return GetRangeForObject((O)relation[id], range);
        }
    }
}
