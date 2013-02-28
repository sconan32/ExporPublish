using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    public interface IPage
    {
        /**
         * Returns the unique id of this Page.
         * 
         * @return the unique id of this Page. Return -1 for unassigned page numbers.
         */
        int GetPageID();

        /**
         * Sets the unique id of this Page.
         * 
         * @param id the id to be set
         */
        void SetPageID(int id);

        /**
         * Returns true if this page is dirty, false otherwise.
         * 
         * @return true if this page is dirty, false otherwise
         */
        bool IsDirty();

        /**
         * Sets the dirty flag of this page.
         * 
         * @param dirty the dirty flag to be set
         */
        void SetDirty(bool dirty);
    }
}
