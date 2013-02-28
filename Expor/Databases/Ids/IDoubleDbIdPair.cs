using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Pair of a double value and a DBID
     * 
     * @author Erich Schubert
     */
    public interface IDoubleDbIdPair : IPair<Double, IDbId>, IDbIdRef, IComparable<IDoubleDbIdPair>
    {
        /**
         * Get the double value of the pair.
         * 
         * @return Double
         */
        double DoubleValue();

    }

}
