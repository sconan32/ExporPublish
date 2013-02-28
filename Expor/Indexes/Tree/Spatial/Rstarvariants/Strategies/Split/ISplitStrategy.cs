using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Split
{

    public interface ISplitStrategy : IParameterizable
    {
        /**
         * Split a page
         * 
         * @param entries Entries to split
         * @param Getter Adapter for the entries array
         * @param minEntries Minimum number of entries in each part
         * @return BitSet containing the assignment.
         */
        BitArray Split<E>(IEnumerable<E> entries, IArrayAdapter Getter, int minEntries) where E : ISpatialComparable;
    }
}
