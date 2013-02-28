using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree
{

    public class IndexTreePath<E> where E : IEntry
    {
        /**
         * Path representing the parent, null if lastPathComponent represents the
         * root.
         */
        private IndexTreePath<E> parentPath;

        /**
         * Last path component.
         */
        private TreeIndexPathComponent<E> lastPathComponent;

        /**
         * Constructs a path from a list of path components, uniquely identifying the
         * path from the root of the index to a specific node. The first element in
         * the path is the root of the index, the last node is the node identified by
         * the path.
         * 
         * @param path a list of IndexPathComponents representing the path to a node
         */
        public IndexTreePath(IList<TreeIndexPathComponent<E>> path)
        {
            if (path == null || path.Count == 0)
            {
                throw new ArgumentException("Path in IndexPath must be non null and not empty.");
            }
            lastPathComponent = path[(path.Count - 1)];
            if (path.Count > 1)
            {
                parentPath = new IndexTreePath<E>(path, path.Count - 1);
            }
        }

        /**
         * Constructs a IndexPath containing only a single element. This is usually
         * used to construct a IndexPath for the the root of the index.
         * 
         * @param singlePath a IndexPathComponent representing the path to a node
         */
        public IndexTreePath(TreeIndexPathComponent<E> singlePath)
        {
            if (singlePath == null)
            {
                throw new ArgumentException("path in TreePath must be non null.");
            }
            lastPathComponent = singlePath;
            parentPath = null;
        }

        /**
         * Constructs a new IndexPath, which is the path identified by
         * <code>parent</code> ending in <code>lastElement</code>.
         * 
         * @param parent the parent path
         * @param lastElement the last path component
         */
        protected IndexTreePath(IndexTreePath<E> parent, TreeIndexPathComponent<E> lastElement)
        {
            if (lastElement == null)
            {
                throw new ArgumentException("path in TreePath must be non null.");
            }
            parentPath = parent;
            lastPathComponent = lastElement;
        }

        /**
         * Constructs a new IndexPath with the identified path components of length
         * <code>length</code>.
         * 
         * @param path the whole path
         * @param length the length of the newly created index path
         */
        protected IndexTreePath(IList<TreeIndexPathComponent<E>> path, int length)
        {
            lastPathComponent = path[(length - 1)];
            if (length > 1)
            {
                parentPath = new IndexTreePath<E>(path, length - 1);
            }
        }

        /**
         * Returns an ordered list of IndexPathComponents containing the components of
         * this IndexPath. The first element (index 0) is the root.
         * 
         * @return an array of IndexPathComponent representing the IndexPath
         */
        public IList<TreeIndexPathComponent<E>> GetPath()
        {
            IList<TreeIndexPathComponent<E>> result = new List<TreeIndexPathComponent<E>>();

            for (IndexTreePath<E> path = this; path != null; path = path.parentPath)
            {
                result.Add(path.lastPathComponent);
            }
            result = (IList<TreeIndexPathComponent<E>>)result.Reverse();
            return result;
        }

        /**
         * Returns the last component of this path.
         * 
         * @return the IndexPathComponent at the end of the path
         */
        public TreeIndexPathComponent<E> GetLastPathComponent()
        {
            return lastPathComponent;
        }

        /**
         * Returns the number of elements in the path.
         * 
         * @return an int giving a count of items the path
         */
        public int GetPathCount()
        {
            int result = 0;
            for (IndexTreePath<E> path = this; path != null; path = path.parentPath)
            {
                result++;
            }
            return result;
        }

        /**
         * Returns the path component at the specified index.
         * 
         * @param element an int specifying an element in the path, where 0 is the
         *        first element in the path
         * @return the Object at that index location
         * @throws ArgumentException if the index is beyond the length of the
         *         path
         */
        public TreeIndexPathComponent<E> GetPathComponent(int element)
        {
            int pathLength = GetPathCount();

            if (element < 0 || element >= pathLength)
            {
                throw new ArgumentException("Index " + element + " is out of the specified range");
            }

            IndexTreePath<E> path = this;

            for (int i = pathLength - 1; i != element; i--)
            {
                path = path.parentPath;
            }
            return path.lastPathComponent;
        }

        /**
         * Returns <code>true</code> if <code>this == o</code> has the value
         * <code>true</code> or o is not null and o is of the same class as this
         * instance and the two index paths are of the same length, and contain the
         * same components (<code>.Equals</code>), <code>false</code> otherwise.
         * 
         * @see de.lmu.ifi.dbs.elki.index.tree.TreeIndexPathComponent#Equals(Object)
         */


        public override bool Equals(Object o)
        {
            if (o == this)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            IndexTreePath<E> other = (IndexTreePath<E>)o;

            if (GetPathCount() != other.GetPathCount())
            {
                return false;
            }
            for (IndexTreePath<E> path = this; path != null; path = path.parentPath)
            {
                if (!(path.lastPathComponent.Equals(other.lastPathComponent)))
                {
                    return false;
                }
                other = other.parentPath;
            }
            return true;
        }

        /**
         * Returns the hash code for this index path. The hash code of a TreeIndexPath
         * is defined to be the hash code of the last component in the path.
         * 
         * @return the hash code of the last component in the index path
         */

        public override int GetHashCode()
        {
            return lastPathComponent.GetHashCode();
        }

        /**
         * Returns true if <code>aIndexPath</code> is a descendant of this IndexPath.
         * A IndexPath P1 is a descendent of a IndexPath P2 if P1 contains all of the
         * components that make up P2's path. For example, if this object has the path
         * [a, b], and <code>aIndexPath</code> has the path [a, b, c], then
         * <code>aIndexPath</code> is a descendant of this object. However, if
         * <code>aIndexPath</code> has the path [a], then it is not a descendant of
         * this object.
         * 
         * @param aIndexPath the index path to be tested
         * @return true if <code>aIndexPath</code> is a descendant of this path
         */
        public bool IsDescendant(IndexTreePath<E> aIndexPath)
        {
            if (aIndexPath == this)
            {
                return true;
            }

            if (aIndexPath != null)
            {
                int pathLength = GetPathCount();
                int oPathLength = aIndexPath.GetPathCount();

                if (oPathLength < pathLength)
                {
                    // Can't be a descendant, has fewer components in the path.
                    return false;
                }
                while (oPathLength-- > pathLength)
                {
                    aIndexPath = aIndexPath.GetParentPath();
                }
                return Equals(aIndexPath);
            }
            return false;
        }

        /**
         * Returns a new path containing all the elements of this object plus
         * <code>child</code>. <code>child</code> will be the last element of the
         * newly created IndexPath. This will throw a NullReferenceException if child is
         * null.
         * 
         * @param child the last element of the newly created IndexPath
         * @return a new path containing all the elements of this object plus
         *         <code>child</code>
         */
        public IndexTreePath<E> PathByAddingChild(TreeIndexPathComponent<E> child)
        {
            if (child == null)
            {
                throw new NullReferenceException("Null child not allowed");
            }

            return new IndexTreePath<E>(this, child);
        }

        /**
         * Returns a path containing all the elements of this object, except the last
         * path component.
         * 
         * @return a path containing all the elements of this object, except the last
         *         path component
         */
        public IndexTreePath<E> GetParentPath()
        {
            return parentPath;
        }

        /**
         * Returns a string that displays the components of this index path.
         * 
         * @return a string representation of the components of this index path
         */

        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder("[");

            for (int counter = 0, maxCounter = GetPathCount(); counter < maxCounter; counter++)
            {
                if (counter > 0)
                {
                    buffer.Append(", ");
                }
                buffer.Append(GetPathComponent(counter));
            }
            buffer.Append("]");
            return buffer.ToString();
        }
    }

}
