using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants.Deliclu
{

    public interface IDeLiCluEntry : ISpatialEntry
    {
        /**
         * Returns true, if the node (or its child nodes) which is represented by this entry
         * contains handled data objects.
         *
         * @return true, if the node (or its child nodes) which is represented by this entry
         *         contains handled data objects,
         *         false otherwise.
         */
         bool HasHandled();

        /**
         * Returns true, if the node (or its child nodes) which is represented by this entry
         * contains unhandled data objects.
         *
         * @return true, if the node (or its child nodes) which is represented by this entry
         *         contains unhandled data objects,
         *         false otherwise.
         */
         bool HasUnhandled();

        /**
         * Sets the flag to marks the node (or its child nodes) which is represented by this entry
         * to contain handled data objects.
         *
         * @param hasHandled the flag to be set
         */
         void SetHasHandled(bool hasHandled);

        /**
         * Sets the flag to marks the node (or its child nodes) which is represented by this entry
         * to contain unhandled data objects.
         *
         * @param hasUnhandled the flag to be set
         */
         void SetHasUnhandled(bool hasUnhandled);
    }

}
