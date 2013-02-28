using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Distances.DistanceFuctions
{


    public interface IDbIdDistanceFunction : IDistanceFunction
    {
        /**
         * Returns the distance between the two objects specified by their object ids.
         * 
         * @param id1 first object id
         * @param id2 second object id
         * @return the distance between the two objects specified by their object ids
         */
        IDistanceValue Distance(IDbIdRef id1, IDbIdRef id2);
    }
}
