using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Databases.Queries.KnnQueries;

namespace Socona.Expor.Indexes
{
  
public interface IKNNIndex :IIndex {
  /**
   * Get a KNN query object for the given distance query and k.
   * 
   * This function MAY return null, when the given distance is not supported!
   * 
   * @param <D> Distance type
   * @param distanceQuery Distance query
   * @param hints Hints for the optimizer
   * @return KNN Query object or {@code null}
   */
  IKNNQuery GetKNNQuery(IDistanceQuery distanceQuery, params Object[] hints);
}
}
