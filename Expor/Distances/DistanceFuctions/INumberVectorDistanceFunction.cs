using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;

namespace Socona.Expor.Distances.DistanceFuctions
{

    /// <summary>
    /// Base interface for the common case of distance functions defined on numerical vectors.
    /// </summary>
    public interface INumberVectorDistanceFunction : IPrimitiveDistanceFunction<INumberVector>
    {
        // Empty - marker interface
    }

}
