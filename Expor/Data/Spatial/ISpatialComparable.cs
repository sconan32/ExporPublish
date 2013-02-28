using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Data.Spatial
{
    /// <summary>
    /// Defines the required methods needed for comparison of spatial objects.
    /// </summary>
    public interface ISpatialComparable : IHasDimensionality
    {
        /// <summary>
        /// Returns the minimum coordinate at the specified dimension.
        /// </summary>
        /// <param name="dimension">
        /// the dimension for which the coordinate should be returned, where 1 < dimension < <code>Count</code>
        /// </param>
        /// <returns>return the minimum coordinate at the specified dimensio</returns>
        double GetMin(int dimension);


        /// <summary>
        /// Returns the maximum coordinate at the specified dimension.
        /// </summary>
        /// <param name="dimension">
        /// the dimension for which the coordinate should be returned,where 1 < dimension < <code>Count</code>
        /// </param>
        /// <returns>return the maximum coordinate at the specified dimension</returns>
        double GetMax(int dimension);
    }
}
