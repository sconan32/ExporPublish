using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Utilities.Parameter;

namespace Socona.Clustering.Algorithms.Clustering.Kmeans
{
    interface IKmeans
    {
        /**
   * Parameter to specify the initialization method
   */
  public static sealed ParameterDescription INIT_ID = ParameterDescription.GetOrCreate("kmeans.initialization", "Method to choose the initial means.");

  /**
   * Parameter to specify the number of clusters to find, must be an integer
   * greater than 0.
   */
  public static sealed ParameterDescription K_ID = ParameterDescription.GetOrCreate("kmeans.k", "The number of clusters to find.");

  /**
   * Parameter to specify the number of clusters to find, must be an integer
   * greater or equal to 0, where 0 means no limit.
   */
  public static sealed ParameterDescription MAXITER_ID = ParameterDescription.GetOrCreate("kmeans.maxiter", "The maximum number of iterations to do. 0 means no limit.");

  /**
   * Parameter to specify the random generator seed.
   */
  public static sealed ParameterDescription SEED_ID = ParameterDescription.GetOrCreate("kmeans.seed", "The random number generator seed.");
}
    }
}
