using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Hierarchy
{

    public class HierarchyReferenceLists<O> : IHierarchy<O>
    where O : IHierarchical<O>
    {
        /**
         * Owner
         */
        protected O owner;

        /**
         * Storage for children
         */
        protected IList<O> children;

        /**
         * Storage for parents
         */
        protected IList<O> parents;

        /**
         * Constructor for hierarchy object.
         * 
         * @param owner owning cluster.
         * @param children child clusters. May be null.
         * @param parents parent clusters. May be null.
         */
        public HierarchyReferenceLists(O owner, IList<O> children, IList<O> parents)
            : base()
        {
            this.owner = owner;
            this.children = children ;
            this.parents = parents;
        }


        public  int NumChildren(O self)
        {
            if (!owner.Equals(self))
            {
                throw new InvalidOperationException("Decentral hierarchy queried for wrong object!");
            }
            if (children == null)
            {
                return 0;
            }
            return children.Count;
        }


        public  IList<O> GetChildren(O self)
        {
            if (!owner.Equals(self))
            {
                throw new InvalidOperationException("Decentral hierarchy queried for wrong object!");
            }
            return children;
        }


        //public Iterator<O> iterDescendants(O self) {
        //  if(owner != self) {
        //    return EmptyIterator.STATIC();
        //  }
        //  if (children == null) {
        //    return EmptyIterator.STATIC();
        //  }
        //  return new ItrDesc(self);
        //}


        public  int NumParents(O self)
        {
            if (!owner.Equals(self))
            {
                throw new InvalidOperationException("Decentral hierarchy queried for wrong object!");
            }
            if (parents == null)
            {
                return 0;
            }
            return parents.Count;
        }

        /**
         * Return parents
         */

        public  IList<O> GetParents(O self)
        {
            if (!owner.Equals(self))
            {
                throw new InvalidOperationException("Decentral hierarchy queried for wrong object!");
            }
            return parents;
        }

        //@Override
        //public Iterator<O> iterAncestors(O self) {
        //  if(owner != self) {
        //    throw new InvalidOperationException("Decentral hierarchy queried for wrong object!");
        //  }
        //  if (parents == null) {
        //    return EmptyIterator.STATIC();
        //  }
        //  return new ItrAnc(self);
        //}

        /**
         * Iterator to collect into the descendants.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        //private class ItrDesc implements Iterator<O> {
        //  /**
        //   * Iterator over children
        //   */
        //  final Iterator<O> childiter;

        //  /**
        //   * Iterator of current child
        //   */
        //  Iterator<O> subiter;

        //  public ItrDesc(O start) {
        //    Debug.Assert (start == owner);
        //    this.childiter = children.iterator();
        //    this.subiter = null;
        //  }

        //  @Override
        //  public boolean hasNext() {
        //    if(subiter != null && subiter.hasNext()) {
        //      return true;
        //    }
        //    return childiter.hasNext();
        //  }

        //  @Override
        //  public O next() {
        //    // Try nested iterator first ...
        //    if(subiter != null && subiter.hasNext()) {
        //      return subiter.next();
        //    }
        //    // Next direct child, update subiter.
        //    final O child = childiter.next();
        //    subiter = child.iterDescendants();
        //    return child;
        //  }

        //  @Override
        //  public void remove() {
        //    throw new InvalidOperationException();
        //  }
        //}

        /**
         * Iterator over all Ancestors.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        //private class ItrAnc implements Iterator<O> {
        //  /**
        //   * Iterator over parents
        //   */
        //  final Iterator<O> parentiter;

        //  /**
        //   * Iterator of current parent
        //   */
        //  Iterator<O> subiter;

        //  public ItrAnc(O start) {
        //    Debug.Assert (start == owner);
        //    this.parentiter = parents.iterator();
        //    this.subiter = null;
        //  }

        //  @Override
        //  public boolean hasNext() {
        //    if(subiter != null && subiter.hasNext()) {
        //      return true;
        //    }
        //    return parentiter.hasNext();
        //  }

        //  @Override
        //  public O next() {
        //    // Try nested iterator first ...
        //    if(subiter != null && subiter.hasNext()) {
        //      return subiter.next();
        //    }
        //    // Next direct parent, update subiter.
        //    final O parent = parentiter.next();
        //    subiter = parent.iterAncestors();
        //    return parent;
        //  }


        //  public void void remove() {
        //    throw new InvalidOperationException();
        //  }
        //}


        public IEnumerable<O> Descendants(O self)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<O> Ancestors(O self)
        {
            throw new NotImplementedException();
        }
    }
}