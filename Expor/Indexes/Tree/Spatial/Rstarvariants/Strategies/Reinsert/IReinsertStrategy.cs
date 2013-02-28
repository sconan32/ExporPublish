using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.DataStructures.ArrayLike;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Reinsert
{

    public interface IReinsertStrategy
    {
        /**
         * Perform reinsertions.
         * 
         * @param entries Entries in overflowing node
         * @param getter Adapter for the entries array
         * @param page Spatial extend of the page
         * @return index of pages to reinsert.
         */
        int[] ComputeReinserts(IEnumerable<ISpatialEntry> entries, IArrayAdapter getter, ISpatialComparable page);
    }

}
