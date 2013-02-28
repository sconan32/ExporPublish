using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;
using Socona.Log;

namespace Socona.Expor.Indexes.Tree.Spatial.Rstarvariants
{

    public abstract class AbstractRStarTreeNode<N, E> : AbstractNode<E>, ISpatialNode<N, E>
        where N : AbstractRStarTreeNode<N, E>
        where E : ISpatialEntry
    {
        /**
         * Empty constructor for Externalizable interface.
         */
        public AbstractRStarTreeNode()
            : base()
        {

        }

        /**
         * Creates a new AbstractRStarTreeNode with the specified parameters.
         * 
         * @param capacity the capacity (maximum number of entries plus 1 for
         *        overflow) of this node
         * @param isLeaf indicates whether this node is a leaf node
         * @param eclass Entry class, to initialize array storage
         */
        public AbstractRStarTreeNode(int capacity, bool isLeaf, Type eclass)
            : base(capacity, isLeaf, eclass)
        {
        }

        /**
         * Recomputing the MBR is rather expensive.
         * 
         * @return MBR
         */
        public ModifiableHyperBoundingBox ComputeMBR()
        {
            E firstEntry = GetEntry(0);
            if (firstEntry == null)
            {
                return null;
            }
            // Note: we deliberately Get a cloned copy here, since we will modify it.
            ModifiableHyperBoundingBox mbr = new ModifiableHyperBoundingBox(firstEntry);
            for (int i = 1; i < numEntries; i++)
            {
                mbr.extend(GetEntry(i));
            }
            return mbr;
        }

        /**
         * Adjusts the parameters of the entry representing this node.
         * 
         * @param entry the entry representing this node
         */
        public virtual bool AdjustEntry(E entry)
        {
            SpatialDirectoryEntry se = entry as SpatialDirectoryEntry;
            ModifiableHyperBoundingBox mbr = ComputeMBR();
            bool changed = false;
            if (se.HasMBR())
            {
                int dim = se.Count;
                // Test for changes
                for (int i = 1; i <= dim; i++)
                {
                    if (Math.Abs(se.GetMin(i) - mbr.GetMin(i)) > float.Epsilon)
                    {
                        changed = true;
                        break;
                    }
                    if (Math.Abs(se.GetMax(i) - mbr.GetMax(i)) > float.Epsilon)
                    {
                        changed = true;
                        break;
                    }
                }
            }
            else
            { // No preexisting MBR.
                changed = true;
            }
            if (changed)
            {
                se.SetMBR(mbr);
            }
            return changed;
        }

        /**
         * Adjusts the parameters of the entry representing this node. Only applicable
         * if one object was inserted or the size of an existing node increased.
         * 
         * @param entry the entry representing this node
         * @param responsibleMBR the MBR of the object or node which is responsible
         *        for the call of the method
         * @return true when the entry has changed
         */
        public bool AdjustEntryIncremental(E entry, ISpatialComparable responsibleMBR)
        {
            return (entry as SpatialDirectoryEntry).ExtendMBR(responsibleMBR);
        }

        /**
         * Tests this node (for debugging purposes).
         */

        public void integrityCheck(AbstractRStarTree<N, E> tree)
        {
            // leaf node
            if (IsLeaf())
            {
                for (int i = 0; i < GetCapacity(); i++)
                {
                    E e = GetEntry(i);
                    if (i < GetNumEntries() && e == null)
                    {
                        throw new ApplicationException("i < numEntries && entry == null");
                    }
                    if (i >= GetNumEntries() && e != null)
                    {
                        throw new ApplicationException("i >= numEntries && entry != null");
                    }
                }
            }

            // dir node
            else
            {
                N tmp = tree.GetNode(GetEntry(0));
                bool childIsLeaf = tmp.IsLeaf();

                for (int i = 0; i < GetCapacity(); i++)
                {
                    E e = GetEntry(i);

                    if (i < GetNumEntries() && e == null)
                    {
                        throw new ApplicationException("i < numEntries && entry == null");
                    }

                    if (i >= GetNumEntries() && e != null)
                    {
                        throw new ApplicationException("i >= numEntries && entry != null");
                    }

                    if (e != null)
                    {
                        N node = tree.GetNode(e);

                        if (childIsLeaf && !node.IsLeaf())
                        {
                            for (int k = 0; k < GetNumEntries(); k++)
                            {
                                tree.GetNode(GetEntry(k));
                            }

                            throw new ApplicationException("Wrong Child in " + this + " at " + i);
                        }

                        if (!childIsLeaf && node.IsLeaf())
                        {
                            throw new ApplicationException("Wrong Child: child id no leaf, but node is leaf!");
                        }

                        node.integrityCheckParameters((N)this, i);
                        node.integrityCheck(tree);
                    }
                }

                //if(LoggingConfiguration.DEBUG) {
                Logging.GetLogger(this.GetType().Name).Debug("DirNode " + GetPageID() + " ok!");
                // }
            }
        }

        /**
         * Tests, if the parameters of the entry representing this node, are correctly
         * set. Subclasses may need to overwrite this method.
         * 
         * @param parent the parent holding the entry representing this node
         * @param index the index of the entry in the parents child array
         */
        protected void integrityCheckParameters(N parent, int index)
        {
            // test if mbr is correctly set
            E entry = parent.GetEntry(index);
            HyperBoundingBox mbr = ComputeMBR();

            if (/* entry.GetMBR() == null && */mbr == null)
            {
                return;
            }
            if (!SpatialUtil.Equals(entry, mbr))
            {
                String soll = mbr.ToString();
                String ist = new HyperBoundingBox(entry).ToString();
                throw new ApplicationException("Wrong MBR in node " + parent.GetPageID() + " at index " + index + " (child " + entry + ")" + "\nsoll: " + soll + ",\n ist: " + ist);
            }
        }

        /**
         * Calls the base method and writes the id of this node, the numEntries and
         * the entries array to the specified stream.
         */

        //public void writeExternal(MemoryStream sout) {
        //  base.writeExternal(sout);
        //  // TODO: do we need to write/read the capacity?
        //  sout.writeInt(entries.Length);
        //  foreach(E entry in entries) {
        //    if(entry == null) {
        //      break;
        //    }
        //    entry.writeExternal(sout);
        //  }
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


        //public void readExternal(ObjectInput sin){
        //  base.readExternal(sin);

        //  // TODO: do we need to write/read the capacity?
        //   int capacity = sin.readInt();
        //  if(IsLeaf()) {
        //    entries = (E[]) new SpatialPointLeafEntry[capacity];
        //    for(int i = 0; i < numEntries; i++) {
        //      SpatialPointLeafEntry s = new SpatialPointLeafEntry();
        //      s.readExternal(sin);
        //      entries[i] = (E) s;
        //    }
        //  }
        //  else {
        //    entries = (E[]) new SpatialDirectoryEntry[capacity];
        //    for(int i = 0; i < numEntries; i++) {
        //      SpatialDirectoryEntry s = new SpatialDirectoryEntry();
        //      s.readExternal(sin);
        //      entries[i] = (E) s;
        //    }
        //  }
        //}
    }
}
