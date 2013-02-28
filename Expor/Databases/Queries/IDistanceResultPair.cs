using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Databases.Queries
{



    /// <summary>
    /// Class that consists of a pair (distance, object ID) commonly returned for kNN and range queries.
    /// </summary>
    public interface IDistanceResultPair : IPair<IDistanceValue, IDbId>, IComparable<IDistanceResultPair>, IDbIdRef
    {
        /**
         * Getter for first
         * 
         * @return first element in pair
         */
        IDistanceValue GetDistance();

        /**
         * Setter for first
         * 
         * @param first new value for first element
         */
        void SetDistance(IDistanceValue first);

        /**
         * Setter for second
         * 
         * @param second new value for second element
         */
        new IDbId DbId { get; set; }

        /**
         * Compare value, but by distance only.
         * 
         * @param o Other object
         * @return comparison result, as by Double.compare(this, other)
         */
        int CompareByDistance(IDistanceResultPair o);
    }
}
