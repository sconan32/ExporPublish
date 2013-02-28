using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Distances.DistanceValues;

namespace Socona.Expor.Databases.Ids.Distance
{
    /**
      * Pair containing a double distance a DBID.
      * 
      * There is no getter for the DBID, as this is a {@link DBIDRef} already.
      * 
      * @author Erich Schubert
      */
    public interface IDoubleDistanceDbIdPair : IDistanceDbIdPair
    {
        /**
         * Get the distance.
         * 
         * @return Distance
         */
        double DoubleDistance();
    }
}
