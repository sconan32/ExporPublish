using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Data;

namespace Socona.Expor.Databases.Queries.SimilarityQueries
{

    /// <summary>
    /// A similarity query serves as adapter layer for database and primitive similarity functions.
    /// </summary>

    public interface ISimilarityQuery : IDatabaseQuery
    {
        /// <summary>
        /// Returns the similarity between the two objects specified by their object ids.
        /// </summary>
        /// <param name="id1">first object id</param>
        /// <param name="id2">second object id</param>
        /// <returns>the similarity between the two objects specified by their object ids</returns>
        IDistanceValue Similarity(IDbIdRef id1, IDbIdRef id2);

        /// <summary>
        /// Returns the similarity between the two objects specified by their object ids.
        /// </summary>
        /// <param name="o1">first object</param>
        /// <param name="id2">second object id</param>
        /// <returns>the similarity between the two objects specified by their object ids</returns>
        IDistanceValue Similarity(IDataVector o1, IDbIdRef id2);


        /// <summary>
        /// Returns the similarity between the two objects specified by their object ids.
        /// </summary>
        /// <param name="id1">first object id</param>
        /// <param name="o2">second object</param>
        /// <returns>the similarity between the two objects specified by their object ids</returns>
        IDistanceValue Similarity(IDbIdRef id1, IDataVector o2);

        /// <summary>
        /// Returns the similarity between the two objects specified by their object ids.
        /// </summary>
        /// <param name="o1">first object</param>
        /// <param name="o2">second object</param>
        /// <returns>the similarity between the two objects specified by their object ids</returns>
        IDistanceValue Similarity(IDataVector o1, IDataVector o2);

        /// <summary>
        /// Method to get the distance functions factory.
        /// </summary>
        IDistanceValue DistanceFactory { get; }


    }
}
