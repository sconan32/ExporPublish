using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Bulk
{

    public interface IBulkSplit : IParameterizable
    {
        /**
         * Partitions the specified feature vectors
         * 
         * @param <T> actual type we split
         * @param spatialObjects the spatial objects to be partitioned
         * @param minEntries the minimum number of entries in a partition
         * @param maxEntries the maximum number of entries in a partition
         * @return the partition of the specified spatial objects
         */
         IList<IList<T>> Partition<T>(IList<T> spatialObjects, int minEntries, int maxEntries);
    }
}
