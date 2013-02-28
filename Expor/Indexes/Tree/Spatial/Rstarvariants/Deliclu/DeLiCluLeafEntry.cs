using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Deliclu
{

    public class DeLiCluLeafEntry : SpatialPointLeafEntry, IDeLiCluEntry
    {
        //private static long serialVersionUID = 1;

        /**
         * Indicates that the node (or its child nodes) which is represented by this entry
         * Contains handled data objects.
         */
        private bool hasHandled;

        /**
         * Indicates that the node (or its child nodes) which is represented by this entry
         * Contains unhandled data objects.
         */
        private bool hasUnhandled;

        /**
         * Empty constructor for serialization purposes.
         */
        public DeLiCluLeafEntry()
        {
            // empty constructor
        }

        /**
         * Constructs a new LeafEntry object with the given parameters.
         *
         * @param id     the unique id of the underlying data object
         * @param vector the vector to store
         */
        public DeLiCluLeafEntry(IDbId id, INumberVector vector) :
            base(id, vector)
        {
            this.hasHandled = false;
            this.hasUnhandled = true;
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
