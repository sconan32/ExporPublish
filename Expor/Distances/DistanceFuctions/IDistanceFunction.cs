using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities.Options;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Data.Spatial;

namespace Socona.Expor.Distances.DistanceFuctions
{


    /// <summary>
    /// Base interface for any kind of distances.
    /// </summary>
    /// <typeparam name="O"> Object type</typeparam>
    public interface IDistanceFunction : IParameterizable
    {
        /**
         * Method to get the distance functions factory.
         * 
         * @return Factory for distance objects
         */
        IDistanceValue DistanceFactory { get; }

        /**
         * Is this function symmetric?
         * 
         * @return {@code true} when symmetric
         */
        bool IsSymmetric { get; }

        /**
         * Is this distance function metric (in particular, does it satisfy the
         * triangle equation?)
         * 
         * @return {@code true} when metric.
         */
        bool IsMetric { get; }

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
        IDistanceQuery Instantiate(IRelation relation);
    }
}
