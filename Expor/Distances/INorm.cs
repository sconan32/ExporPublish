using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances
{
    
public interface INorm<O> : IDistanceFunction {
  /**
   * Compute the norm of object obj.
   * 
   * @param obj Object
   * @return Norm
   */
  IDistanceValue Norm(O obj);
}
}
