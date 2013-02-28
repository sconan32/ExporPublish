using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Queries.DistanceQueries;
using Socona.Expor.Indexes;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public interface IIndexBasedDistanceFunction : IDistanceFunction
    {
        /**
         * OptionDescription for the index parameter
         */
        //static OptionDescription INDEX_ID = OptionDescription.GetOrCreate("distancefunction.index", "Distance index to use.");

        /**
         * Instance interface for Index based distance functions.
         * 
         * @author Erich Schubert
         * 
         * @param <T> Object type
         * @param <D> Distance type
         */

    }
    public interface IIndexBasedDistanceFunctionInstance : IDistanceQuery
    {
        /**
         * Get the index used.
         * 
         * @return the index used
         */
        IIndex GetIndex();
    }
}
