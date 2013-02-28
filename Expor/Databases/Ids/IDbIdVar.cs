using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{
    /**
     * (Persistent) variable storing a DBID reference.
     * 
     * In contrast to the {@link DBIDRef} API, which are read-only references, this
     * variable can be updated to point to a different DBID, e.g. the current best
     * candidate.
     * 
     * @author Erich Schubert
     */
    public interface IDbIdVar : IDbIdRef, IArrayDbIds, ISetDbIds
    {
        /**
         * Assign a new value for the reference.
         * 
         * @param ref Reference
         */
        void Set(IDbIdRef dbidref);
    }

}
