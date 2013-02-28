using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.Options;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Insert
{

    public interface IInsertionStrategy : IParameterizable
    {
        /**
         * Choose insertion rectangle.
         * 
         * @param options Options to choose from
         * @param Getter Array adapter for options
         * @param obj Insertion object
         * @param height Tree height
         * @param depth Insertion depth (depth == height - 1 indicates leaf level)
         * @return Subtree index in array.
         */
         int Choose(IEnumerable<ISpatialEntry> options, IArrayAdapter getter, ISpatialComparable obj, int height, int depth);
    }
}
