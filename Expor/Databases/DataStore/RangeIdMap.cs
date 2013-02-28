using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{

    public class RangeIdMap : IDataStoreIdMap
    {
        /**
         * Start offset
         */
        IDbIdRange range;

        /**
         * Constructor from a static DBID range allocation.
         * 
         * @param range DBID range to use
         */
        public RangeIdMap(IDbIdRange range)
        {
            this.range = range;
        }


        public  int Map(IDbIdRef dbid)
        {
            return range.GetOffset(dbid);
        }
    }

}
