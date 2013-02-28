using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Hierarchy
{

    /**
     * Modifiable Hierarchy.
     * 
     * @author Erich Schubert
     * 
     * @param <O> Object type
     */
    public interface IModifiableHierarchy<O> : IHierarchy<O>
    {
        /**
         * Add a parent-child relationship.
         * 
         * @param parent Parent
         * @param child Child
         */
        // TODO: return true when new?
        void Add(O parent, O child);
        /**
      * Add an entry (initializes data structures).
      * 
      * @param entry Entry
      */
        void Add(O entry);

        /**
         * Remove a parent-child relationship.
         * 
         * @param parent Parent
         * @param child Child
         */
        // TODO: return true when found?
        void Remove(O parent, O child);
        /**
      * Remove an entry and all its parent-child relationships.
      * 
      * @param entry Entry
      */
        void Remove(O entry);

        /**
         * Remove an entry and it's whole subtree (unless the elements are reachable
         * by a different path!)
         * 
         * @param entry Entry
         */
        void RemoveSubtree(O entry);
    }

}
