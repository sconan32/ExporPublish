using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree.Spatial
{

    public interface ISpatialNode<N, E> : INode<E>
        where N : ISpatialNode<N, E>
        where E : ISpatialEntry
    {
        // No Additional methods.
    }
}
