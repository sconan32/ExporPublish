using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Indexes;

namespace Socona.Expor.Distances.SimilarityFunctions
{

    public interface IIndexBasedSimilarityFunction<O> : ISimilarityFunction
    {
        /**
         * Preprocess the database to Get the actual distance function.
         * 
         * @param database
         * @return Actual distance query.
         */

       // IIndexBasedSimilarityFunctionInstance<O> Instantiate(IRelation database);
    }
    /**
     * Instance interface for index/preprocessor based distance functions.
     * 
     * @author Erich Schubert
     * 
     * @param <T> Object type
     * @param <IDistance> Distance type
     */
    public interface IIndexBasedSimilarityFunctionInstance<T> : ISimilarityQuery
    {
        /**
         * Get the index used.
         * 
         * @return the index used
         */
        IIndex GetIndex();
    }

}
