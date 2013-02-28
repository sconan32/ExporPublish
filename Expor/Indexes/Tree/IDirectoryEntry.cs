using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree
{

    public interface IDirectoryEntry : IEntry
    {
        /**
         * Returns the id of the node or data object that is represented by this
         * entry.
         * 
         * @return the id of the node or data object that is represented by this entry
         */
        int GetEntryID();

        /**
         * Get the page ID of this leaf entry.
         */
         int GetPageID();
    }
}
