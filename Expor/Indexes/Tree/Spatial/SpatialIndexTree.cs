using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;

namespace Socona.Expor.Indexes.Tree.Spatial
{

    public abstract class SpatialIndexTree<N, E> : IndexTree<N, E>
        where N : ISpatialNode<N, E>
        where E : ISpatialEntry
    {
        /**
         * Constructor.
         * 
         * @param pagefile Page file
         */
        public SpatialIndexTree(IPageFile<N> pagefile) :
            base(pagefile)
        {
        }

        /**
         * Add a new leaf entry to the tree.
         * 
         * @param leaf Leaf entry
         */
        public abstract void InsertLeaf(E leaf);

        /**
         * Returns a list of entries pointing to the leaf entries of this spatial
         * index.
         * 
         * @return a list of entries pointing to the leaf entries of this spatial
         *         index
         */
        public abstract IList<E> GetLeaves();
    }
}
