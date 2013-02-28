using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Strategies.Overflow
{

    public interface IOverflowTreatment
    {
        /**
         * Reinitialize the reinsertion treatment (for a new primary insertion).
         */
        void Reinitialize();

        /**
         * Handle overflow in the given node.
         * 
         * @param <N> Node
         * @param <E> Entry
         * @param tree Tree
         * @param node Node
         * @param path Path
         * @return true when already handled (e.g. by reinserting)
         */
        bool HandleOverflow<N, E>(AbstractRStarTree<N, E> tree, N node, IndexTreePath<E> path)
            where N : AbstractRStarTreeNode<N, E>
            where E : ISpatialEntry;
    }
}
