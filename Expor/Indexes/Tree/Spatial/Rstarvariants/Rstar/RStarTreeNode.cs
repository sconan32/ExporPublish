using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Rstar
{

    /**
     * Represents a node in an R*-Tree.
     * 
     * @author Elke Achtert
     */
    public class RStarTreeNode : AbstractRStarTreeNode<RStarTreeNode, ISpatialEntry>
    {
     //   private static long serialVersionUID = 1;

        /**
         * Empty constructor for Externalizable interface.
         */
        public RStarTreeNode()
        {
            // empty constructor
        }

        /**
         * Creates a new RStarTreeNode with the specified parameters.
         * 
         * @param capacity the capacity (maximum number of entries plus 1 for
         *        overflow) of this node
         * @param isLeaf indicates whether this node is a leaf node
         */
        public RStarTreeNode(int capacity, bool isLeaf) :
            base(capacity, isLeaf, typeof(ISpatialEntry))
        { }
    }
}
