using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Types;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions
{

    /**
     * Primitive distance function that is defined on some kind of object.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     * 
     * @param <O> input object type
     * @param <D> distance result type
     */
    public interface IPrimitiveDistanceFunction<in O> : IDistanceFunction
    {

        /// <summary>
        /// Computes the distance between two given DatabaseObjects according to this distance function.
        /// </summary>
        /// <param name="o1">first DatabaseObject(INumberVector?)</param>
        /// <param name="o2">second DatabaseObject</param>
        /// <returns></returns>
        IDistanceValue Distance(O o1, O o2);



    }
}
