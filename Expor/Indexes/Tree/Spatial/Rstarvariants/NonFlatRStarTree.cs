using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants
{

    public abstract class NonFlatRStarTree<N, E> : AbstractRStarTree<N, E>
        where N : AbstractRStarTreeNode<N, E>
        where E : ISpatialEntry
    {
        /**
         * Constructor.
         * 
         * @param pagefile Page file
         */
        public NonFlatRStarTree(IPageFile<N> pagefile) :
            base(pagefile)
        {
        }

        /**
         * Returns true if in the specified node an overflow occurred, false
         * otherwise.
         * 
         * @param node the node to be tested for overflow
         * @return true if in the specified node an overflow occurred, false otherwise
         */

        protected override bool HasOverflow(N node)
        {
            if (node.IsLeaf())
            {
                return node.GetNumEntries() == leafCapacity;
            }
            else
            {
                return node.GetNumEntries() == dirCapacity;
            }
        }

        /**
         * Returns true if in the specified node an underflow occurred, false
         * otherwise.
         * 
         * @param node the node to be tested for underflow
         * @return true if in the specified node an underflow occurred, false
         *         otherwise
         */

        protected override bool HasUnderflow(N node)
        {
            if (node.IsLeaf())
            {
                return node.GetNumEntries() < leafMinimum;
            }
            else
            {
                return node.GetNumEntries() < dirMinimum;
            }
        }

        /**
         * Computes the height of this RTree. Is called by the constructor. and should
         * be overwritten by subclasses if necessary.
         * 
         * @return the height of this RTree
         */

        protected override int ComputeHeight()
        {
            N node = GetRoot();
            int height = 1;

            // compute height
            while (!node.IsLeaf() && node.GetNumEntries() != 0)
            {
                E entry = node.GetEntry(0);
                node = GetNode(entry);
                height++;
            }
            return height;
        }


        protected override void CreateEmptyRoot(E exampleLeaf)
        {
            N root = CreateNewLeafNode();
            WriteNode(root);
            SetHeight(1);
        }

        /**
         * Performs a bulk load on this RTree with the specified data. Is called by
         * the constructor and should be overwritten by subclasses if necessary.
         */

        protected override void BulkLoad(IList<E> spatialObjects)
        {
            if (!initialized)
            {
                Initialize(spatialObjects[(0)]);
            }

            StringBuilder msg = GetLogger().IsDebugging ? new StringBuilder() : null;

            // Tiny tree that fits into a single page
            if (spatialObjects.Count <= leafCapacity)
            {
                N root = CreateNewLeafNode();
                root.SetPageID(GetRootID());
                WriteNode(root);
                CreateRoot(root, spatialObjects);
                SetHeight(1);
                if (msg != null)
                {
                    msg.Append("\n  numNodes = 1");
                }
            }
            // root is directory node
            else
            {
                N root = CreateNewDirectoryNode();
                root.SetPageID(GetRootID());
                WriteNode(root);

                // create leaf nodes
                IList<E> nodes = CreateBulkLeafNodes(spatialObjects);

                int numNodes = nodes.Count;
                if (msg != null)
                {
                    msg.Append("\n  numLeafNodes = ").Append(numNodes);
                }
                SetHeight(1);

                // create directory nodes
                while (nodes.Count > (dirCapacity - 1))
                {
                    nodes = CreateBulkDirectoryNodes(nodes);
                    numNodes += nodes.Count;
                    SetHeight(GetHeight() + 1);
                }

                // create root
                CreateRoot(root, nodes);
                numNodes++;
                SetHeight(GetHeight() + 1);
                if (msg != null)
                {
                    msg.Append("\n  numNodes = ").Append(numNodes);
                }
            }
            if (msg != null)
            {
                msg.Append("\n  height = ").Append(GetHeight());
                msg.Append("\n  root " + GetRoot());
                GetLogger().Debug(msg.ToString() + "\n");
            }
        }

        /**
         * Creates and returns the directory nodes for bulk load.
         * 
         * @param nodes the nodes to be inserted
         * @return the directory nodes containing the nodes
         */
        private IList<E> CreateBulkDirectoryNodes(IList<E> nodes)
        {
            int minEntries = dirMinimum;
            int maxEntries = dirCapacity - 1;

            IList<E> result = new List<E>();
            IList<IList<E>> partitions = bulkSplitter.Partition(nodes, minEntries, maxEntries);

            foreach (List<E> partition in partitions)
            {
                // create node
                N dirNode = CreateNewDirectoryNode();
                // insert nodes
                foreach (E o in partition)
                {
                    dirNode.AddDirectoryEntry(o);
                }
                // write to file
                WriteNode(dirNode);

                result.Add(CreateNewDirectoryEntry(dirNode));
                if (GetLogger().IsDebugging)
                {
                    GetLogger().Debug("Directory page no: " + dirNode.GetPageID());
                }
            }

            return result;
        }

        /**
         * Returns a root node for bulk load. If the objects are data objects a leaf
         * node will be returned, if the objects are nodes a directory node will be
         * returned.
         * 
         * @param root the new root node
         * @param objects the spatial objects to be inserted
         * @return the root node
         */
        private N CreateRoot(N root, IList<E> objects)
        {
            // insert data
            foreach (E entry in objects)
            {
                if (entry.IsLeafEntry())
                {
                    root.AddLeafEntry(entry);
                }
                else
                {
                    root.AddDirectoryEntry(entry);
                }
            }

            // set root mbr
            (GetRootEntry() as SpatialDirectoryEntry).SetMBR(root.ComputeMBR());

            // write to file
            WriteNode(root);
            if (GetLogger().IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("pageNo ").Append(root.GetPageID());
                GetLogger().Debug(msg.ToString() + "\n");
            }

            return root;
        }
    }
}
