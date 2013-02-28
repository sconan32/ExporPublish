using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Results
{

    public interface IOrderingResult : IResult
    {
        /**
         * Get the full set of DBIDs this ordering is defined for.
         * 
         * @return DBIDs
         */
        IDbIds GetDbIds();
        IArrayModifiableDbIds Iter(IDbIds ids);
    }

}
