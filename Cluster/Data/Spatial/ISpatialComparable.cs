using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Data.Spatial
{


    /**
     * Defines the required methods needed for comparison of spatial objects.
     * 
     * @author Elke Achtert
     */
    public interface ISpatialComparable
    {
        /**
         * Returns the dimensionality of the object.
         * 
         * @return the dimensionality
         */
        int GetDimensionality();

        /**
         * Returns the minimum coordinate at the specified dimension.
         * 
         * @param dimension the dimension for which the coordinate should be returned,
         *        where 1 &le; dimension &le; <code>getDimensionality()</code>
         * @return the minimum coordinate at the specified dimension
         */
        double GetMin(int dimension);

        /**
         * Returns the maximum coordinate at the specified dimension.
         * 
         * @param dimension the dimension for which the coordinate should be returned,
         *        where 1 &le; dimension &le; <code>getDimensionality()</code>
         * @return the maximum coordinate at the specified dimension
         */
        double GetMax(int dimension);
    }
}
