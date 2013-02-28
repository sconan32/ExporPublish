using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Distance
{
    /**
     * Double-valued KNN result.
     * 
     * @author Erich Schubert
     */
    public interface IDoubleDistanceKNNList : IKNNList
    {
        /**
         * Get the kNN distance as double value.
         * 
         * @return Distance
         */
        double DoubleKNNDistance { get; }

    }

}
