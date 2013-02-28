using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.DistanceQueries
{
    /**
     * Distance query for spatial distance functions
     * @author Erich Schubert
     *
     * @apiviz.uses SpatialPrimitiveDistanceFunction
     * 
     * @param <V> Vector type to use
     * @param <D> Distance result type
     */
    public class SpatialPrimitiveDistanceQuery : PrimitiveDistanceQuery<ISpatialComparable>, ISpatialDistanceQuery
    {
        /**
         * The distance function we use.
         */
        protected new ISpatialPrimitiveDistanceFunction distanceFunction;

        /**
         * @param relation Representation to use
         * @param distanceFunction Distance function to use
         */
        public SpatialPrimitiveDistanceQuery(IRelation relation,
            ISpatialPrimitiveDistanceFunction distanceFunction)
            : base(relation, distanceFunction)
        {
            this.distanceFunction = distanceFunction;
        }


        public IDistanceValue MinDistance(ISpatialComparable mbr, ISpatialComparable v)
        {
            return distanceFunction.MinDistance(mbr, v);
        }

        public IDistanceValue MinDistance(ISpatialComparable mbr, IDbId id)
        {
            return distanceFunction.MinDistance(mbr, (ISpatialComparable)relation[id]);
        }

        public override IDistanceFunction DistanceFunction
        {
            get { return distanceFunction; }
        }
    }
}
