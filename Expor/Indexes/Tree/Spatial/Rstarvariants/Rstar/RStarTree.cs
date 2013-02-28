using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.Documentation;
using Socona.Log;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Rstar
{

    /**
     * RStarTree is a spatial index structure based on the concepts of the R*-Tree.
     * Apart from organizing the objects it also provides several methods to search
     * for certain object in the structure and ensures persistence.
     * 
     * @author Elke Achtert
     * 
     * @apiviz.has RStarTreeNode oneway - - contains
     */
    [Title("R*-Tree")]
    [Description("Balanced index structure based on bounding rectangles.")]
    [Reference(Authors = "N. Beckmann, H.-P. Kriegel, R. Schneider, B. Seeger",
        Title = "The R*-tree: an efficient and robust access method for points and rectangles",
        BookTitle = "Proceedings of the 1990 ACM SIGMOD International Conference on Management of Data, Atlantic City, NJ, May 23-25, 1990",
        Url = "http://dx.doi.org/10.1145/93597.98741")]
    public class RStarTree : NonFlatRStarTree<RStarTreeNode, ISpatialEntry>
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(RStarTree));

        /**
         * Constructor.
         * 
         * @param pagefile Page file
         */
        public RStarTree(IPageFile<RStarTreeNode> pagefile) :
            base(pagefile)
        {
        }


        protected override ISpatialEntry CreateRootEntry()
        {
            return new SpatialDirectoryEntry(0, null);
        }


        protected override ISpatialEntry CreateNewDirectoryEntry(RStarTreeNode node)
        {
            return new SpatialDirectoryEntry(node.GetPageID(), node.ComputeMBR());
        }

        /**
         * Creates a new leaf node with the specified capacity.
         * 
         * @return a new leaf node
         */

        protected override RStarTreeNode CreateNewLeafNode()
        {
            return new RStarTreeNode(leafCapacity, true);
        }

        /**
         * Creates a new directory node with the specified capacity.
         * 
         * @return a new directory node
         */

        protected override RStarTreeNode CreateNewDirectoryNode()
        {
            return new RStarTreeNode(dirCapacity, false);
        }

        protected override Logging GetLogger()
        {
            return logger;
        }
    }
}
