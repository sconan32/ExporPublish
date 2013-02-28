using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Immutable pair of two DBIDs. This can be stored more efficiently than when
     * using {@link de.lmu.ifi.dbs.elki.utilities.pairs.Pair}
     * 
     * @author Erich Schubert
     * 
     * @apiviz.composedOf de.lmu.ifi.dbs.elki.database.ids.DBID
     */
    // TODO: implement DBIDs?
    public interface IDbIdPair : IPair<IDbId, IDbId>
    {
        /**
         * Getter for first
         * 
         * @return first element in pair
         */

        // IDbId GetFirst();

        /**
         * Getter for second element in pair
         * 
         * @return second element in pair
         */

       // IDbId GetSecond();
    }
}
