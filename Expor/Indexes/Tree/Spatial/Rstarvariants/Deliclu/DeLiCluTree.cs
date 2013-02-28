using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using Socona.Log;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Deliclu
{

    public class DeLiCluTree : NonFlatRStarTree<DeLiCluNode, IDeLiCluEntry>
    {
        /**
         * The logger for this class.
         */
        private static Logging logger = Logging.GetLogger(typeof(DeLiCluTree));

        /**
         * Holds the ids of the expanded nodes.
         */
        private Dictionary<Int32, HashSet<Int32>> expanded = new Dictionary<Int32, HashSet<Int32>>();

        /**
         * Constructor.
         * 
         * @param pagefile Page file
         */
        public DeLiCluTree(IPageFile<DeLiCluNode> pagefile) :
            base(pagefile)
        {
        }

        /**
         * Marks the nodes with the specified ids as expanded.
         * 
         * @param entry1 the first node
         * @param entry2 the second node
         */
        public void SetExpanded(ISpatialEntry entry1, ISpatialEntry entry2)
        {
            HashSet<Int32> exp1 = expanded[(GetPageID(entry1))];
            if (exp1 == null)
            {
                exp1 = new HashSet<Int32>();
                expanded[GetPageID(entry1)] = exp1;
            }
            exp1.Add(GetPageID(entry2));
        }

        /**
         * Returns the nodes which are already expanded with the specified node.
         * 
         * @param entry the id of the node for which the expansions should be returned
         * @return the nodes which are already expanded with the specified node
         */
        public ISet<Int32> GetExpanded(ISpatialEntry entry)
        {
            HashSet<Int32> exp = expanded[(GetPageID(entry))];
            if (exp != null)
            {
                return exp;
            }
            return new HashSet<Int32>();
        }

        /**
         * Returns the nodes which are already expanded with the specified node.
         * 
         * @param entry the id of the node for which the expansions should be returned
         * @return the nodes which are already expanded with the specified node
         */
        public ISet<Int32> GetExpanded(DeLiCluNode entry)
        {
            HashSet<Int32> exp = expanded[(entry.GetPageID())];
            if (exp != null)
            {
                return exp;
            }
            return new HashSet<Int32>();
        }

        /**
         * Determines and returns the number of nodes in this index.
         * 
         * @return the number of nodes in this index
         */
        public int NumNodes()
        {
            int numNodes = 0;

            BreadthFirstEnumeration<DeLiCluNode, IDeLiCluEntry> bfs = 
                new BreadthFirstEnumeration<DeLiCluNode, IDeLiCluEntry>(this, GetRootPath());
            while (bfs.MoveNext())
            {
                IEntry entry = bfs.Current.GetLastPathComponent().GetEntry();
                if (!entry.IsLeafEntry())
                {
                    numNodes++;
                }
            }

            return numNodes;
        }

        /**
         * Creates a new leaf node with the specified capacity.
         * 
         * @return a new leaf node
         */

        protected override DeLiCluNode CreateNewLeafNode()
        {
            return new DeLiCluNode(leafCapacity, true);
        }

        /**
         * Creates a new directory node with the specified capacity.
         * 
         * @return a new directory node
         */

        protected override DeLiCluNode CreateNewDirectoryNode()
        {
            return new DeLiCluNode(dirCapacity, false);
        }

        /**
         * Creates a new directory entry representing the specified node.
         * 
         * @param node the node to be represented by the new entry
         */

        protected override IDeLiCluEntry CreateNewDirectoryEntry(DeLiCluNode node)
        {
            return new DeLiCluDirectoryEntry(node.GetPageID(), 
                node.ComputeMBR(), node.HasHandled(), node.HasUnhandled());
        }

        /**
         * Creates an entry representing the root node.
         * 
         * @return an entry representing the root node
         */

        protected override IDeLiCluEntry CreateRootEntry()
        {
            return new DeLiCluDirectoryEntry(0, null, false, true);
        }


        protected override Logging GetLogger()
        {
            return logger;
        }
    }
}
