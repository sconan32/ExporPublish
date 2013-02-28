using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public interface IDoubleNorm<O> : INorm<O>, IPrimitiveDoubleDistanceFunction<O>
    {

        /// <summary>
        /// Compute the norm of object obj as double value.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Double</returns>
        double DoubleNorm(O obj);
    }
}
