using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Deliclu
{

    public class DeLiCluDirectoryEntry : SpatialDirectoryEntry, IDeLiCluEntry
    {
        //private static long serialVersionUID = 1;

        /**
         * Indicates that the node (or its child nodes) which is represented by this
         * entry contains handled data objects.
         */
        private bool hasHandled;

        /**
         * Indicates that the node (or its child nodes) which is represented by this
         * entry contains unhandled data objects.
         */
        private bool hasUnhandled;

        /**
         * Empty constructor for serialization purposes.
         */
        public DeLiCluDirectoryEntry()
        {
            // empty constructor
        }

        /**
         * Constructs a new DeLiCluDirectoryEntry object with the given parameters.
         * 
         * @param id the unique id of the underlying spatial node
         * @param mbr the minimum bounding rectangle of the underlying spatial node
         * @param hasHandled indicates if this entry has handled nodes
         * @param hasUnhandled indicates if this entry has unhandled nodes
         */
        public DeLiCluDirectoryEntry(int id, ModifiableHyperBoundingBox mbr, bool hasHandled, bool hasUnhandled) :
            base(id, mbr)
        {
            this.hasHandled = hasHandled;
            this.hasUnhandled = hasUnhandled;
        }


        public bool HasHandled()
        {
            return hasHandled;
        }


        public bool HasUnhandled()
        {
            return hasUnhandled;
        }


        public void SetHasHandled(bool hasHandled)
        {
            this.hasHandled = hasHandled;
        }


        public void SetHasUnhandled(bool hasUnhandled)
        {
            this.hasUnhandled = hasUnhandled;
        }

        /**
         * Returns the id as a string representation of this entry.
         * 
         * @return a string representation of this entry
         */

        public override String ToString()
        {
            return base.ToString() + "[" + hasHandled + "-" + hasUnhandled + "]";
        }
    }
}
