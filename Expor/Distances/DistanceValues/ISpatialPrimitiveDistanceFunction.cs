using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Databases.Queries.DistanceQueries;
namespace Socona.Expor.Distances.DistanceFuctions
{

    public interface ISpatialPrimitiveDistanceFunction : IPrimitiveDistanceFunction<ISpatialComparable>
    {
        /**
         * Computes the distance between the two given MBRs according to this distance
         * function.
         * 
         * @param mbr1 the first MBR object
         * @param mbr2 the second MBR object
         * @return the distance between the two given MBRs according to this distance
         *         function
         */
        IDistanceValue MinDistance(ISpatialComparable mbr1, ISpatialComparable mbr2);



    }
}
