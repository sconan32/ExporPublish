using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Databases.Ids
{

    /**
     * Interface for DBIDs that support fast "set" operations, in particular
     * "contains" lookups.
     * 
     * @author Erich Schubert
     */
    public interface ISetDbIds : IDbIds
    {
        // empty marker interface
    }
}
