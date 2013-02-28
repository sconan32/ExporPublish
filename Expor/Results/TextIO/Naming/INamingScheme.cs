using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Models;

namespace Socona.Expor.Results.TextIO.Naming
{

    public interface INamingScheme
    {
        /**
         * Retrieve a name for the given cluster.
         * 
         * @param cluster cluster to get a name for
         * @return cluster name
         */
         String GetNameFor(Cluster cluster);
    }

}
