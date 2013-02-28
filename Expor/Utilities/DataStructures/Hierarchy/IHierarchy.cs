using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Hierarchy
{

    /**
     * This interface represents an (external) hierarchy of objects. It can contain
     * arbitrary objects, BUT the hierarchy has to be accessed using the hierarchy
     * object, i.e. {@code hierarchy.getChildren(object);}.
     * 
     * See {@link Hierarchical} for an interface for objects with an internal
     * hierarchy (where you can use {@code object.getChildren();})
     * 
     * @author Erich Schubert
     * 
     * @param <O> Object type
     */
    public interface IHierarchy<O>
    {
        /**
         * Get number of children
         * 
         * @param self object to get number of children for
         * @return number of children
         */
        int NumChildren(O self);

        /**
         * Get children list. Resulting list MAY be modified. Result MAY be null, if
         * the model is not hierarchical.
         * 
         * @param self object to get children for
         * @return list of children
         */
        IList<O> GetChildren(O self);

        /**
         * Iterate descendants (recursive children)
         * 
         * @param self object to get descendants for
         * @return iterator for descendants
         */
        IEnumerable<O> Descendants(O self);

        /**
         * Get number of (direct) parents
         * 
         * @param self reference object
         * @return number of parents
         */
        int NumParents(O self);

        /**
         * Get parents list. Resulting list MAY be modified. Result MAY be null, if
         * the model is not hierarchical.
         * 
         * @param self object to get parents for
         * @return list of parents
         */
        IList<O> GetParents(O self);

        /**
         * Iterate ancestors (recursive parents)
         * 
         * @param self object to get ancestors for
         * @return iterator for ancestors
         */
        IEnumerable<O> Ancestors(O self);
    }
}
