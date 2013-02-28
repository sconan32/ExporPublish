using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Exceptions;
using Socona.Expor.Utilities.Extenstions;
using Socona.Log;
namespace Socona.Expor.Indexes.Tree
{

    public abstract class AbstractNode<E> : AbstractExternalizablePage, INode<E>
        where E : IEntry
    {
        /**
         * The number of entries in this node.
         */
        protected int numEntries;

        /**
         * The entries (children) of this node.
         */
        protected E[] entries;

        /**
         * Indicates whether this node is a leaf node.
         */
        protected bool isLeaf;

        /**
         * Empty constructor for Externalizable interface.
         */
        public AbstractNode() :
            base()
        {
        }

        /**
         * Creates a new Node with the specified parameters.
         * 
         * @param capacity the capacity (maximum number of entries plus 1 for
         *        overflow) of this node
         * @param isLeaf indicates whether this node is a leaf node
         * @param eclass Entry class, to initialize array storage
         */
        public AbstractNode(int capacity, bool isLeaf, Type eclass) :
            base()
        {
            this.numEntries = 0;
            Type cls = ClassGenericsUtil.UglyCastIntoSubclass(eclass);
            this.entries = ClassGenericsUtil.NewArrayOfNull<E>(capacity, cls);
            this.isLeaf = isLeaf;
        }


        public IEnumerable<IndexTreePath<E>> Children(IndexTreePath<E> parentPath)
        {
            for (int i = 0; i < numEntries; i++)
            {
                yield return parentPath.PathByAddingChild(new TreeIndexPathComponent<E>(entries[i], i));
            }

        }


        public int GetNumEntries()
        {
            return numEntries;
        }


        public bool IsLeaf()
        {
            return isLeaf;
        }


        public E GetEntry(int index)
        {
            return entries[index];
        }

        /**
         * Calls the base method and writes the id of this node, the numEntries and
         * the entries array to the specified stream.
         */

        //public void WriteExternal(MemoryStream sout)
        //{
        //    base.WriteExternal(sout);
        //    sout.writeBoolean(isLeaf);
        //    sout.writeInt(numEntries);
        //    // Entries will be written in subclasses
        //}

        ///**
        // * Reads the id of this node, the numEntries and the entries array from the
        // * specified stream.
        // * 
        // * @param in the stream to read data from in order to restore the object
        // * @throws java.io.IOException if I/O errors occur
        // * @throws ClassNotFoundException If the class for an object being restored
        // *         cannot be found.
        // */

        //public void ReadExternal(ObjectInput sin)
        //{
        //    base.readExternal(sin);
        //    isLeaf = sin.readBoolean();
        //    numEntries = sin.readInt();
        //    // Entries will be read in subclasses
        //}

        /**
         * Returns <code>true</code> if <code>this == o</code> has the value
         * <code>true</code> or o is not null and o is of the same class as this
         * instance and <code>base.Equals(o)</code> returns <code>true</code> and
         * both nodes are of the same type (leaf node or directory node) and have
         * contain the same entries, <code>false</code> otherwise.
         * 
         * @see de.lmu.ifi.dbs.elki.persistent.AbstractExternalizablePage#Equals(Object)
         */

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            if (!base.Equals(o))
            {
                return false;
            }

            AbstractNode<E> that = (AbstractNode<E>)o;

            return isLeaf == that.isLeaf && numEntries == that.numEntries && Array.Equals(entries, that.entries);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ isLeaf.GetHashCode() ^
                numEntries.GetHashCode() ^ entries.GetHashCode();
        }

        /**
         * Returns a string representation of this node.
         * 
         * @return the type of this node (LeafNode or DirNode) followed by its id
         */

        public override String ToString()
        {
            if (isLeaf)
            {
                return "LeafNode " + GetPageID();
            }
            else
            {
                return "DirNode " + GetPageID();
            }
        }

        /**
         * Adds a new leaf entry to this node's children and returns the index of the
         * entry in this node's children array. An InvalidOperationException will
         * be thrown if the entry is not a leaf entry or this node is not a leaf node.
         * 
         * @param entry the leaf entry to be Added
         * @return the index of the entry in this node's children array
         * @throws InvalidOperationException if entry is not a leaf entry or this
         *         node is not a leaf node
         */

        public int AddLeafEntry(E entry)
        {
            // entry is not a leaf entry
            if (!entry.IsLeafEntry())
            {
                throw new InvalidOperationException("Entry is not a leaf entry!");
            }
            // this is a not a leaf node
            if (!IsLeaf())
            {
                throw new InvalidOperationException("Node is not a leaf node!");
            }

            // leaf node
            return AddEntry(entry);
        }

        /**
         * Adds a new directory entry to this node's children and returns the index of
         * the entry in this node's children array. An InvalidOperationException
         * will be thrown if the entry is not a directory entry or this node is not a
         * directory node.
         * 
         * @param entry the directory entry to be Added
         * @return the index of the entry in this node's children array
         * @throws InvalidOperationException if entry is not a directory entry or
         *         this node is not a directory node
         */

        public int AddDirectoryEntry(E entry)
        {
            // entry is not a directory entry
            if (entry.IsLeafEntry())
            {
                throw new InvalidOperationException("Entry is not a directory entry!");
            }
            // this is a not a directory node
            if (IsLeaf())
            {
                throw new InvalidOperationException("Node is not a directory node!");
            }

            return AddEntry(entry);
        }

        /**
         * Deletes the entry at the specified index and shifts all entries after the
         * index to left.
         * 
         * @param index the index at which the entry is to be deleted
         * @return true id deletion was successful
         */
        public bool DeleteEntry(int index)
        {
            Array.Copy(entries, index + 1, entries, index, numEntries - index - 1);
            entries[--numEntries] = default(E);
            return true;
        }

        /**
         * Deletes all entries in this node.
         */
        public void DeleteAllEntries()
        {
            if (numEntries > 0)
            {
                Array.ForEach(entries, (e) => { e = default(E); });
                this.numEntries = 0;
            }
        }

        /**
         * Returns the capacity of this node (i.e. the length of the entries arrays).
         * 
         * @return the capacity of this node
         */
        public int GetCapacity()
        {
            return entries.Length;
        }

        /**
         * Returns a list of the entries.
         * 
         * @return a list of the entries
         * 
         * @deprecated Using this method means an extra copy - usually at the cost of
         *             performance.
         */
        [Obsolete]

        public List<E> GetEntries()
        {
            List<E> result = new List<E>(numEntries);
            foreach (E entry in entries)
            {
                if (entry != null)
                {
                    result.Add(entry);
                }
            }
            return result;
        }

        /**
         * Adds the specified entry to the entries array and increases the numEntries
         * counter.
         * 
         * @param entry the entry to be Added
         * @return the current number of entries
         */
        private int AddEntry(E entry)
        {
            entries[numEntries++] = entry;
            return numEntries - 1;
        }

        /**
         * Remove entries according to the given mask.
         * 
         * @param mask Mask to remove
         */
        public void RemoveMask(BitArray mask)
        {
            int dest = mask.NextSetBitIndex(0);
            if (dest < 0)
            {
                return;
            }
            int src = mask.NextClearBit(dest);
            while (src < numEntries)
            {
                if (!mask.Get(src))
                {
                    entries[dest] = entries[src];
                    dest++;
                }
                src++;
            }
            int rm = src - dest;
            while (dest < numEntries)
            {
                entries[dest] = default(E);
                dest++;
            }
            numEntries -= rm;
        }

        /**
         * Redistribute entries according to the given sorting.
         * 
         * @param newNode Node to split to
         * @param sorting Sorting to use
         * @param splitPoint Split point
         */
        public void SplitTo(AbstractNode<E> newNode, IList<E> sorting, int splitPoint)
        {
            Debug.Assert(IsLeaf() == newNode.IsLeaf());
            DeleteAllEntries();
            StringBuilder msg = new StringBuilder("\n");

            for (int i = 0; i < splitPoint; i++)
            {
                AddEntry(sorting[(i)]);
                if (msg != null)
                {
                    msg.Append("n_").Append(GetPageID()).Append(" ");
                    msg.Append(sorting[(i)]).Append("\n");
                }
            }

            for (int i = splitPoint; i < sorting.Count; i++)
            {
                newNode.AddEntry(sorting[(i)]);
                if (msg != null)
                {
                    msg.Append("n_").Append(newNode.GetPageID()).Append(" ");
                    msg.Append(sorting[(i)]).Append("\n");
                }
            }
            if (msg != null)
            {
                Logging.GetLogger(this.GetType().Name).Debug(msg.ToString());
            }
        }

        /**
         * Splits the entries of this node into a new node using the given assignments
         * 
         * @param newNode Node to split to
         * @param assignmentsToFirst the assignment to this node
         * @param assignmentsToSecond the assignment to the new node
         */
        public void SplitTo(AbstractNode<E> newNode, List<E> assignmentsToFirst, List<E> assignmentsToSecond)
        {
            Debug.Assert(IsLeaf() == newNode.IsLeaf());
            DeleteAllEntries();
            StringBuilder msg = new StringBuilder();

            // assignments to this node
            foreach (E entry in assignmentsToFirst)
            {
                if (msg != null)
                {
                    msg.Append("n_").Append(GetPageID()).Append(" ").Append(entry).Append("\n");
                }
                AddEntry(entry);
            }

            // assignments to the new node
            foreach (E entry in assignmentsToSecond)
            {
                if (msg != null)
                {
                    msg.Append("n_").Append(newNode.GetPageID()).Append(" ").Append(entry).Append("\n");
                }
                newNode.AddEntry(entry);
            }
            if (msg != null)
            {
                Logging.GetLogger(this.GetType()).Debug(msg.ToString());
            }
        }

        /**
         * Splits the entries of this node into a new node using the given assignments
         * 
         * @param newNode Node to split to
         * @param assignment Assignment mask
         */
        public void SplitByMask(AbstractNode<E> newNode, BitArray assignment)
        {
            Debug.Assert(IsLeaf() == newNode.IsLeaf());
            int dest = assignment.NextSetBitIndex(0);
            if (dest < 0)
            {
                throw new AbortException("No bits set in splitting mask.");
            }
            int pos = dest;
            while (pos < numEntries)
            {
                if (assignment.Get(pos))
                {
                    // Move to new node
                    newNode.AddEntry(entries[pos]);
                }
                else
                {
                    // Move to new position
                    entries[dest] = entries[pos];
                    dest++;
                }
                pos++;
            }
            int rm = numEntries - dest;
            while (dest < numEntries)
            {
                entries[dest] = default(E);
                dest++;
            }
            numEntries -= rm;
        }

        public IEnumerator<E> GetEnumerator()
        {
            return entries.GetEnumerator() as IEnumerator<E>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }
    }
}
