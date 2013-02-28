using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Queries.DistanceQueries
{

    public interface ISpatialDistanceQuery : IDistanceQuery 
    {
        /**
         * Computes the minimum distance between the given MBR and the FeatureVector
         * object according to this distance function.
         * 
         * @param mbr the MBR object
         * @param v the FeatureVector object
         * @return the minimum distance between the given MBR and the FeatureVector
         *         object according to this distance function
         */
        IDistanceValue MinDistance(ISpatialComparable mbr, ISpatialComparable v);

        /**
         * Computes the minimum distance between the given MBR and the FeatureVector
         * object according to this distance function.
         * 
         * @param mbr the MBR object
         * @param id the query object id
         * @return the minimum distance between the given MBR and the FeatureVector
         *         object according to this distance function
         */
        IDistanceValue MinDistance(ISpatialComparable mbr, IDbId id);

        /**
         * Get the inner distance function.
         * 
         * @return Distance function
         */

        //ISpatialPrimitiveDistanceFunction<V> DistanceFunction { get; }
    }
}
