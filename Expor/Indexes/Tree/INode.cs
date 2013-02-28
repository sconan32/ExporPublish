using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;

namespace Socona.Expor.Indexes.Tree
{

    public interface INode<E> : IPage, IEnumerable<E> where E : IEntry
    {
        /**
         * Returns an enumeration of the children paths of this node.
         * 
         * @param parentPath the path to this node
         * @return an enumeration of the children paths of this node
         */
        IEnumerable<IndexTreePath<E>> Children(IndexTreePath<E> parentPath);

        /**
         * Returns the number of entries of this node.
         * 
         * @return the number of entries of this node
         */
        int GetNumEntries();

        /**
         * Returns true if this node is a leaf node, false otherwise.
         * 
         * @return true if this node is a leaf node, false otherwise
         */
        bool IsLeaf();

        /**
         * Returns the entry at the specified index.
         * 
         * @param index the index of the entry to be returned
         * @return the entry at the specified index
         */
        E GetEntry(int index);

        /**
         * Adds a new leaf entry to this node's children and returns the index of the
         * entry in this node's children array. An UnsupportedOperationException will
         * be thrown if the entry is not a leaf entry or this node is not a leaf node.
         * 
         * @param entry the leaf entry to be Added
         * @return the index of the entry in this node's children array
         * @throws UnsupportedOperationException if entry is not a leaf entry or this
         *         node is not a leaf node
         */
        int AddLeafEntry(E entry);

        /**
         * Adds a new directory entry to this node's children and returns the index of
         * the entry in this node's children array. An UnsupportedOperationException
         * will be thrown if the entry is not a directory entry or this node is not a
         * directory node.
         * 
         * @param entry the directory entry to be Added
         * @return the index of the entry in this node's children array
         * @throws UnsupportedOperationException if entry is not a directory entry or
         *         this node is not a directory node
         */
        int AddDirectoryEntry(E entry);
    }
}