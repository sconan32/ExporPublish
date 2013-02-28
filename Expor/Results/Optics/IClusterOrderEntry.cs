using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Results.Optics
{

    public interface IClusterOrderEntry
    {
        /**
         * Returns the object id of this entry.
         * 
         * @return the object id of this entry
         */
         IDbId GetID();

        /**
         * Returns the id of the predecessor of this entry if this entry has a
         * predecessor, null otherwise.
         * 
         * @return the id of the predecessor of this entry
         */
         IDbId GetPredecessorID();

        /**
         * Returns the reachability distance of this entry
         * 
         * @return the reachability distance of this entry
         */
         IDistanceValue GetReachability();
    }
}
