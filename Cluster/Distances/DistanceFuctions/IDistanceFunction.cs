using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Data.Types;
using Socona.Clustering.Databases.Relations;
using Socona.Clustering.Utilities.Options;
using Socona.Clustering.Distances.DistanceValues;
using Socona.Clustering.Databases.Queries.DistanceQueries;

namespace Socona.Clustering.Distances.DistanceFuctions
{

    /**
     * Base interface for any kind of distances.
     * 
     * @author Erich Schubert
     *
     * @param <O> Object type
     * @param <D> Distance result type
     * 
     * @apiviz.landmark
     * @apiviz.has Distance
     */
    public interface IDistanceFunction<O, D> : IParameterizable where D : IDistance
    {
        /**
         * Method to get the distance functions factory.
         * 
         * @return Factory for distance objects
         */
        D GetDistanceFactory();

        /**
         * Is this function symmetric?
         * 
         * @return {@code true} when symmetric
         */
        bool IsSymmetric();

        /**
         * Is this distance function metric (in particular, does it satisfy the
         * triangle equation?)
         * 
         * @return {@code true} when metric.
         */
        bool IsMetric();

        /**
         * Get the input data type of the function.
         * 
         * @return Type restriction
         */
        ITypeInformation GetInputTypeRestriction();

        /**
         * Instantiate with a database to get the actual distance query.
         * 
         * @param relation The representation to use
         * @return Actual distance query.
         */
        public IDistanceQuery<T, D> Instantiate<T>(IRelation<T> relation);
    }
}
