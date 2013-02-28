using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceFuctions;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Data;

namespace Socona.Expor.Databases.Queries.DistanceQueries
{
    /// <summary>
    /// A distance query serves as adapter layer for database and primitive distances.
    /// </summary>
    public interface IDistanceQuery : IDatabaseQuery
    {

        /// <summary>
        /// Returns the distance between the two objects specified by their object ids.
        /// </summary>
        /// <param name="id1">first object id</param>
        /// <param name="id2">second object id</param>
        /// <returns>the distance between the two objects specified by their object ids</returns>
        IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2);

        /// <summary>
        /// Returns the distance between the two objects specified by their object ids.
        /// </summary>
        /// <param name="o1">first object</param>
        /// <param name="id2">second object id</param>
        /// <returns>the distance between the two objects specified by their object ids</returns>
        IDistanceValue Distance(IDataVector o1, IDbIdRef id2);

        /// <summary>
        /// Returns the distance between the two objects specified by their object ids.
        /// </summary>
        /// <param name="id1">first object id</param>
        /// <param name="o2">second object</param>
        /// <returns> the distance between the two objects specified by their object ids</returns>
        IDistanceValue Distance(IDbIdRef id1, IDataVector o2);


        /// <summary>
        ///  Returns the distance between the two objects specified by their object ids.
        /// </summary>
        /// <param name="o1" first object></param>
        /// <param name="o2">second object</param>
        /// <returns>the distance between the two objects specified by their object ids</returns>
        IDistanceValue Distance(IDataVector o1, IDataVector o2);

        /// <summary>
        /// get the distance functions factory.
        /// </summary>
        IDistanceValue DistanceFactory { get; }

        /// <summary>
        /// Get the inner distance function.
        /// </summary>
        IDistanceFunction DistanceFunction { get; }

        /// <summary>
        /// Provides an infinite distance.
        /// </summary>
        IDistanceValue Infinity { get; }

        /// <summary>
        /// Provides a null distance.
        /// </summary>
        IDistanceValue Empty { get; }

        /// <summary>
        /// Provides an undefined distance.
        /// </summary>
        IDistanceValue Undefined { get; }

    }
}
