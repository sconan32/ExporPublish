using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public interface ISpatialPrimitiveDoubleDistanceFunction :
        ISpatialPrimitiveDistanceFunction
    {
        /**
         * Computes the distance between the two given MBRs according to this
         * distance function.
         * 
         * @param mbr1 the first MBR object
         * @param mbr2 the second MBR object
         * @return the distance between the two given MBRs according to this
         *         distance function
         */
        double MinDoubleDistance(ISpatialComparable mbr1, ISpatialComparable mbr2);
    }
}