using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions
{

    /**
     * Interface for distance functions that can provide a raw double value.
     * 
     * This is for use in performance-critical situations that need to avoid the
     * boxing/unboxing cost of regular distance API.
     * 
     * @author Erich Schubert
     * 
     * @param <O> Object type
     */
    public interface IPrimitiveDoubleDistanceFunction<in O> : IPrimitiveDistanceFunction<O>
    {

        /// <summary>
        /// Computes the distance between two given Objects according to this distance function.
        /// </summary>
        /// <param name="o1">first Object(INumberVector?)</param>
        /// <param name="o2">second Object</param>
        /// <returns>the distance between two given Objects according to this distance function</returns>
        double DoubleDistance(O o1, O o2);
    }
}
