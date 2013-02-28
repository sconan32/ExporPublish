using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Relations;

namespace Socona.Expor.Distances.DistanceFuctions
{

    public interface IFilteredLocalPCABasedDistanceFunction<O> : IIndexBasedDistanceFunction
    {
        /**
         * Instantiate with a database to Get the actual distance query.
         * 
         * @param database
         * @return Actual distance query.
         */

      //   IFilteredLocalPCABasedDistanceFunctionInstance Instantiate(IRelation database);

        /**
         * Instance produced by the distance function.
         * 
         * @author Erich Schubert
         * 
         * @param <T> Database object type
         * @param <I> Index type
         * @param <D> Distance type
         */

    }
    public interface IFilteredLocalPCABasedDistanceFunctionInstance : IIndexBasedDistanceFunctionInstance
    {
        // No Additional restrictions
    }
}
