using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Ids.Distance;
using Socona.Expor.Databases.Ids.Generic;
using Socona.Expor.Databases.Ids.Int32DbIds;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RangeQueries
{

    public class LinearScanRangeQuery<O> : AbstractDistanceRangeQuery<O>, ILinearScanQuery
        where O:IDataVector
    {
        /**
         * Constructor.
         * 
         * @param distanceQuery Distance function to use
         */
        public LinearScanRangeQuery(IDistanceQuery distanceQuery) :
            base(distanceQuery)
        {
        }


        public override IDistanceDbIdList GetRangeForDbId(IDbIdRef id, IDistanceValue range)
        {
            GenericDistanceDbIdList result = new GenericDistanceDbIdList();
            foreach (var id2 in relation.GetDbIds())
            {
                IDistanceValue currentDistance = distanceQuery.Distance(id, id2);
                if (currentDistance.CompareTo(range) <= 0)
                {
                    result.Add(new DistanceInt32DbIdPair(currentDistance, id2.Int32Id));
                }
            }
            result.Sort();
            return result;
        }


        public override IDistanceDbIdList GetRangeForObject(O obj, IDistanceValue range)
        {
            GenericDistanceDbIdList result = new GenericDistanceDbIdList();
            foreach (var id in relation.GetDbIds())
            {
                IDistanceValue currentDistance = distanceQuery.Distance(obj, id);
                if (currentDistance.CompareTo(range) <= 0)
                {
                    result.Add(new DistanceInt32DbIdPair(currentDistance, id.Int32Id));
                }
            }
            result.Sort();
            return result;
        }
    }
}
