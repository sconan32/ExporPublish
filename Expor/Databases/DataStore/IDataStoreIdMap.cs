using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Databases.DataStore
{

    public interface IDataStoreIdMap
    {
        /**
         * Map a DBID to a database id.
         * 
         * @param dbid DBID
         * @return record id {@code id >= 0}
         */
        int Map(IDbIdRef dbid);
    }

}
