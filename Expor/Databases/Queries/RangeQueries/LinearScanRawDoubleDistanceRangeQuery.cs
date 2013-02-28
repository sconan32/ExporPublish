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
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.RangeQueries
{

    public class LinearScanRawDoubleDistanceRangeQuery<O> : LinearScanRangeQuery<O>, ILinearScanQuery
        where O:IDataVector
    {
        /**
         * Constructor.
         * 
         * @param distanceQuery Distance function to use
         */
        public LinearScanRawDoubleDistanceRangeQuery(IDistanceQuery distanceQuery) :
            base(distanceQuery)
        {
        }


        public IDistanceDbIdList GetRangeForDbId(IDbIdRef id, DoubleDistanceValue range)
        {
            if (distanceQuery is PrimitiveDistanceQuery<O> && distanceQuery.DistanceFunction is IPrimitiveDoubleDistanceFunction<O>)
            {

                IPrimitiveDoubleDistanceFunction<O> rawdist = (IPrimitiveDoubleDistanceFunction<O>)distanceQuery.DistanceFunction;
                double epsilon = range.DoubleValue();

                O qo = (O)relation[id];
                GenericDistanceDbIdList result = new GenericDistanceDbIdList();
                foreach (var id2 in relation.GetDbIds())
                {
                    double doubleDistance = rawdist.DoubleDistance(qo, (O)relation[id2]);
                    if (doubleDistance <= epsilon)
                    {
                        result.Add(new DoubleDistanceInt32DbIdPair(doubleDistance, id2.Int32Id));
                    }
                }
                result.Sort();
                return result;
            }
            else
            {
                return base.GetRangeForDbId(id, range);
            }
        }


        public IDistanceDbIdList getRangeForObject(O obj, DoubleDistanceValue range)
        {
            if (distanceQuery is PrimitiveDistanceQuery<O> &&
                distanceQuery.DistanceFunction is IPrimitiveDoubleDistanceFunction<O>)
            {

                IPrimitiveDoubleDistanceFunction<O> rawdist = (IPrimitiveDoubleDistanceFunction<O>)distanceQuery.DistanceFunction;
                double epsilon = range.DoubleValue();

                GenericDistanceDbIdList result = new GenericDistanceDbIdList();
                foreach (var id2 in relation.GetDbIds())
                {
                    double doubleDistance = rawdist.DoubleDistance(obj, (O)relation[id2]);
                    if (doubleDistance <= epsilon)
                    {
                        result.Add(new DoubleDistanceInt32DbIdPair(doubleDistance, id2.Int32Id));
                    }
                }
                result.Sort();
                return (IDistanceDbIdList)result;
            }
            else
            {
                return base.GetRangeForObject(obj, range);
            }
        }
    }
}
