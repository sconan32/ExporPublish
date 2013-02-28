using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Types;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Databases.Queries.SimilarityQueries;
using Socona.Expor.Databases.Relations;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Distances.SimilarityFunctions
{

    /**
     * Interface SimilarityFunction describes the requirements of any similarity
     * function.
     * 
     * @author Elke Achtert
     * 
     * @apiviz.landmark
     * @apiviz.has Distance
     * 
     * @param <O> object type
     * @param <D> distance type
     */
    public interface ISimilarityFunction : IParameterizable
    {
        /**
         * Is this function symmetric?
         * 
         * @return {@code true} when symmetric
         */
        bool IsSymmetric();

        /**
         * Get the input data type of the function.
         */
        ITypeInformation GetInputTypeRestriction();

        /**
         * Get a distance factory.
         * 
         * @return distance factory
         */
        IDistanceValue GetDistanceFactory();

        /**
         * Instantiate with a representation to get the actual similarity query.
         * 
         * @param relation Representation to use
         * @return Actual distance query.
         */
        ISimilarityQuery Instantiate(IRelation relation);
    }
}
