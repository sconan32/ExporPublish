using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Databases.Ids.Distance
{

    /**
     * Pair containing a distance an an object ID
     * 
     * Note: there is no getter for the object, as this is a {@link DBIDRef}.
     * 
     * @author Erich Schubert
     *
     * @param <D> Distance
     */
    public interface IDistanceDbIdPair : IDbIdRef
    {
        /**
         * Get the distance.
         * 
         * @return Distance
         */
        IDistanceValue Distance { get; }

        /**
         * Compare to another result, by distance, smaller first.
         * 
         * @param other Other result
         * @return Comparison result
         */
        int CompareByDistance(IDistanceDbIdPair other);
    }

}
