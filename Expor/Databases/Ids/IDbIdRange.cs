using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Static DBID range.
     * 
     * @author Erich Schubert
     */
    public interface IDbIdRange : IArrayStaticDbIds
    {
        /**
         * Get offset in the array for a particular DBID.
         * 
         * Should satisfy {@code range.get(getOffset(id)) == id} and
         * {@code range.getOffset(range.get(idx)) == idx}. 
         * 
         * @param dbid ID to compute index for
         * @return index
         */
      int GetOffset(IDbIdRef dbid);
    }
}
