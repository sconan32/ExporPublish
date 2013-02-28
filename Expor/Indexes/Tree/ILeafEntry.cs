using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Indexes.Tree
{

    public interface ILeafEntry : IEntry
    {
        /**
         * Get the DbId of this leaf entry.
         */
         IDbId GetDbId();
    }

}
