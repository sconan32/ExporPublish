using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Hierarchy
{

    public interface IHierarchical<O> 
    {
        /**
         * Test for hierarchical properties
         * 
         * @return hierarchical data model.
         */
         bool IsHierarchical();

        /**
         * Get number of children
         * 
         * @return number of children
         */
         int NumChildren();

        /**
         * Get children list. Resulting list MAY be modified. Result MAY be null, if
         * the model is not hierarchical.
         * 
         * @return list of children
         */
        IList<O> GetChildren();

        
        /**
         * Iterate descendants (recursive children)
         * 
         * @return iterator for descendants
         */
        //public Iterator<O> iterDescendants();

        /**
         * Get number of parents
         * 
         * @return number of parents
         */
        int NumParents();

        /**
         * Get parents list. Resulting list MAY be modified. Result MAY be null, if
         * the model is not hierarchical.
         * 
         * @return list of parents
         */
         IList<O> GetParents();

        /**
         * Iterate ancestors (recursive parents)
         * 
         * @return iterator for ancestors
         */
        //public Iterator<O> iterAncestors();
    }
}
