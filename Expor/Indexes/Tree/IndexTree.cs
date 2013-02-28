using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.Exceptions;
using Socona.Log;

namespace Socona.Expor.Indexes.Tree
{

    public abstract class IndexTree<N, E>
        where N : INode<E>
        where E : IEntry
    {
        /**
         * The file storing the entries of this index.
         */
        private IPageFile<N> file;

        /**
         * True if this index is already initialized.
         */
        protected bool initialized = false;

        /**
         * The capacity of a directory node (= 1 + maximum number of entries in a
         * directory node).
         */
        protected int dirCapacity;

        /**
         * The capacity of a leaf node (= 1 + maximum number of entries in a leaf
         * node).
         */
        protected int leafCapacity;

        /**
         * The minimum number of entries in a directory node.
         */
        protected int dirMinimum;

        /**
         * The minimum number of entries in a leaf node.
         */
        protected int leafMinimum;

        /**
         * The entry representing the root node.
         */
        private E rootEntry;

        /**
         * Constructor.
         * 
         * @param pagefile page file to use
         */
        public IndexTree(IPageFile<N> pagefile) :
            base()
        {
            this.file = pagefile;
        }

        /**
         * Initialize the tree if the page file already existed.
         */
        // FIXME: ensure this is called in all the appropriate places!
        public virtual void Initialize()
        {
            TreeIndexHeader header = CreateHeader();
            if (this.file.Initialize(header))
            {
                InitializeFromFile(header, file);
            }
            rootEntry = CreateRootEntry();
        }

        /**
         * Get the (STATIC) logger for this class.
         * 
         * @return the static logger
         */
        abstract protected Logging GetLogger();

        /**
         * Returns the entry representing the root if this index.
         * 
         * @return the entry representing the root if this index
         */
        public E GetRootEntry()
        {
            return rootEntry;
        }

        /**
         * Page ID of the root entry.
         * 
         * @return page id
         */
        public int GetRootID()
        {
            return GetPageID(rootEntry);
        }

        /**
         * Reads the root node of this index from the file.
         * 
         * @return the root node of this index
         */
        public N GetRoot()
        {
            return file.ReadPage(GetPageID(rootEntry));
        }

        /**
         * Test if a given ID is the root.
         * 
         * @param page Page to test
         * @return Whether the page ID is the root
         */
        protected bool IsRoot(N page)
        {
            return GetRootID() == page.GetPageID();
        }

        /**
         * Convert a directory entry to its page id.
         * 
         * @param entry Entry
         * @return Page ID
         */
        protected virtual int GetPageID(IEntry entry)
        {
            if (entry.IsLeafEntry())
            {
                throw new AbortException("Leafs do not have page ids!");
            }
            return ((IDirectoryEntry)entry).GetPageID();
        }

        /**
         * Returns the node with the specified id.
         * 
         * @param nodeID the page id of the node to be returned
         * @return the node with the specified id
         */
        public N GetNode(int nodeID)
        {
            if (nodeID == GetPageID(rootEntry))
            {
                return GetRoot();
            }
            else
            {
                return file.ReadPage(nodeID);
            }
        }

        /**
         * Returns the node that is represented by the specified entry.
         * 
         * @param entry the entry representing the node to be returned
         * @return the node that is represented by the specified entry
         */
        public N GetNode(E entry)
        {
            return GetNode(GetPageID(entry));
        }

        /**
         * Write a node to the backing storage.
         * 
         * @param node Node to write
         */
        protected void WriteNode(N node)
        {
            file.WritePage(node);
        }

        /**
         * Delete a node from the backing storage.
         * 
         * @param node Node to delete
         */
        protected void DeleteNode(N node)
        {
            file.DeletePage(node.GetPageID());
        }

        /**
         * Creates a header for this index structure which is an instance of
         * {@link TreeIndexHeader}. Subclasses may need to overwrite this method if
         * they need a more specialized header.
         * 
         * @return a new header for this index structure
         */
        protected TreeIndexHeader CreateHeader()
        {
            return new TreeIndexHeader(file.GetPageSize(), dirCapacity, leafCapacity, dirMinimum, leafMinimum);
        }

        /**
         * Initializes this index from an existing persistent file.
         */
        public virtual void InitializeFromFile(TreeIndexHeader header, IPageFile<N> file)
        {
            this.dirCapacity = header.GetDirCapacity();
            this.leafCapacity = header.GetLeafCapacity();
            this.dirMinimum = header.GetDirMinimum();
            this.leafMinimum = header.GetLeafMinimum();

            if (GetLogger().IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append(GetType());
                msg.Append("\n file = ").Append(file.GetType());
                GetLogger().Debug(msg.ToString());
            }

            this.initialized = true;
        }

        /**
         * Initializes the index.
         * 
         * @param exampleLeaf an object that will be stored in the index
         */
        protected void Initialize(E exampleLeaf)
        {
            InitializeCapacities(exampleLeaf);

            // create empty root
            CreateEmptyRoot(exampleLeaf);

            if (GetLogger().IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append(GetType()).Append("\n");
                msg.Append(" file    = ").Append(file.GetType()).Append("\n");
                msg.Append(" maximum number of dir entries = ").Append((dirCapacity - 1)).Append("\n");
                msg.Append(" minimum number of dir entries = ").Append(dirMinimum).Append("\n");
                msg.Append(" maximum number of leaf entries = ").Append((leafCapacity - 1)).Append("\n");
                msg.Append(" minimum number of leaf entries = ").Append(leafMinimum).Append("\n");
                msg.Append(" root    = ").Append(GetRoot());
                GetLogger().Debug(msg.ToString());
            }

            initialized = true;
        }

        /**
         * Returns the path to the root of this tree.
         * 
         * @return the path to the root of this tree
         */
        public IndexTreePath<E> GetRootPath()
        {
            return new IndexTreePath<E>(new TreeIndexPathComponent<E>(rootEntry, 0));
        }

        /**
         * Determines the maximum and minimum number of entries in a node.
         * 
         * @param exampleLeaf an object that will be stored in the index
         */
        protected abstract void InitializeCapacities(E exampleLeaf);

        /**
         * Creates an empty root node and writes it to file.
         * 
         * @param exampleLeaf an object that will be stored in the index
         */
        protected abstract void CreateEmptyRoot(E exampleLeaf);

        /**
         * Creates an entry representing the root node.
         * 
         * @return an entry representing the root node
         */
        protected abstract E CreateRootEntry();

        /**
         * Creates a new leaf node with the specified capacity.
         * 
         * @return a new leaf node
         */
        protected abstract N CreateNewLeafNode();

        /**
         * Creates a new directory node with the specified capacity.
         * 
         * @return a new directory node
         */
        protected abstract N CreateNewDirectoryNode();

        /**
         * Performs necessary operations before inserting the specified entry.
         * 
         * @param entry the entry to be inserted
         */
        protected virtual void PreInsert(E entry)
        {
            // Default is no-op.
        }

        /**
         * Performs necessary operations after deleting the specified entry.
         * 
         * @param entry the entry that was removed
         */
        protected virtual void PostDelete(E entry)
        {
            // Default is no-op.
        }

        /**
         * Get the index file page access statistics.
         * 
         * @return access statistics
         */
        public IPageFileStatistics GetPageFileStatistics()
        {
            return file;
        }

        /**
         * Get the page size of the backing storage.
         * 
         * @return Page size
         */
        protected int GetPageSize()
        {
            return file.GetPageSize();
        }

        /**
         * Directly access the backing page file.
         * 
         * @return the page file
         */
        [Obsolete]
        protected IPageFile<N> GetFile()
        {
            return file;
        }
    }
}
