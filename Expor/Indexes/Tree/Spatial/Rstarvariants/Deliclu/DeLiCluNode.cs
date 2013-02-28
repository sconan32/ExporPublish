using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Deliclu
{

    public class DeLiCluNode : AbstractRStarTreeNode<DeLiCluNode, IDeLiCluEntry>
    {
       // private static long serialVersionUID = 1;

        /**
         * Empty constructor for Externalizable interface.
         */
        public DeLiCluNode()
        {
            // empty constructor
        }

        /**
         * Creates a new DeLiCluNode with the specified parameters.
         * 
         * @param capacity the capacity (maximum number of entries plus 1 for
         *        overflow) of this node
         * @param isLeaf indicates whether this node is a leaf node
         */
        public DeLiCluNode(int capacity, bool isLeaf) :
            base(capacity, isLeaf, typeof(IDeLiCluEntry))
        {
        }

        /**
         * Returns true, if the children of this node (or their child nodes) contain
         * handled data objects.
         * 
         * @return true, if the children of this node (or their child nodes) contain
         *         handled data objects
         */
        public bool HasHandled()
        {
            for (int i = 1; i < GetNumEntries(); i++)
            {
                bool handled = GetEntry(i).HasHandled();
                if (handled)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Returns true, if the children of this node (or their child nodes) contain
         * unhandled data objects.
         * 
         * @return true, if the children of this node (or their child nodes) contain
         *         unhandled data objects
         */
        public bool HasUnhandled()
        {
            for (int i = 1; i < GetNumEntries(); i++)
            {
                bool handled = GetEntry(i).HasUnhandled();
                if (handled)
                {
                    return true;
                }
            }
            return false;
        }


        public override bool AdjustEntry(IDeLiCluEntry entry)
        {
            bool changed = base.AdjustEntry(entry);
            // adjust hasHandled and hasUnhandled flag
            bool hasHandled = HasHandled();
            bool hasUnhandled = HasUnhandled();
            entry.SetHasHandled(hasHandled);
            entry.SetHasUnhandled(hasUnhandled);
            return changed;
        }

        /**
         * Tests, if the parameters of the entry representing this node, are correctly
         * set. Subclasses may need to overwrite this method.
         * 
         * @param parent the parent holding the entry representing this node
         * @param index the index of the entry in the parents child array
         */

        protected void IntegrityCheckParameters(DeLiCluNode parent, int index)
        {
            base.integrityCheckParameters(parent, index);
            // test if hasHandled and hasUnhandled flag are correctly set
            IDeLiCluEntry entry = parent.GetEntry(index);
            bool hasHandled = HasHandled();
            bool hasUnhandled = HasUnhandled();
            if (entry.HasHandled() != hasHandled)
            {
                String soll = hasHandled.ToString();
                String ist = entry.HasHandled().ToString();
                throw new ApplicationException("Wrong hasHandled in node " + parent.GetPageID() + 
                    " at index " + index + " (child " + entry + ")" + "\nsoll: " + soll + ",\n ist: " + ist);
            }
            if (entry.HasUnhandled() != hasUnhandled)
            {
                String soll = hasUnhandled.ToString();
                String ist = entry.HasUnhandled().ToString();
                throw new ApplicationException("Wrong hasUnhandled in node " + parent.GetPageID() + 
                    " at index " + index + " (child " + entry + ")" + "\nsoll: " + soll + ",\n ist: " + ist);
            }
        }
    }
}
