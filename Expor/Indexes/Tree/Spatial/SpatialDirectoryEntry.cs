using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Data.Spatial;

namespace Socona.Expor.Indexes.Tree.Spatial
{

    public class SpatialDirectoryEntry : AbstractDirectoryEntry, ISpatialEntry
    {
       // private static long serialVersionUID = 1;

        /**
         * The minimum bounding rectangle of the underlying spatial node.
         */
        private ModifiableHyperBoundingBox mbr;

        /**
         * Empty constructor for serialization purposes.
         */
        public SpatialDirectoryEntry()
        {
            // empty constructor
        }

        /**
         * Constructs a new SpatialDirectoryEntry object with the given parameters.
         * 
         * @param id the unique id of the underlying spatial node
         * @param mbr the minimum bounding rectangle of the underlying spatial node
         */
        public SpatialDirectoryEntry(int id, ModifiableHyperBoundingBox mbr) :
            base(id)
        {
            this.mbr = mbr;
        }


        public int Count
        {
            get { return mbr.Count; }
        }

        /**
         * @return the coordinate at the specified dimension of the minimum hyper
         *         point of the MBR of the underlying node
         */

        public double GetMin(int dimension)
        {
            return mbr.GetMin(dimension);
        }

        /**
         * @return the coordinate at the specified dimension of the maximum hyper
         *         point of the MBR of the underlying node
         */

        public double GetMax(int dimension)
        {
            return mbr.GetMax(dimension);
        }

        /**
         * Test whether this entry already has an MBR.
         * 
         * @return True when an MBR exists.
         */
        public bool HasMBR()
        {
            return (this.mbr != null);
        }

        /**
         * Sets the MBR of this entry.
         * 
         * @param mbr the MBR to be set
         */
        public void SetMBR(ModifiableHyperBoundingBox mbr)
        {
            this.mbr = mbr;
        }

        /**
         * Calls the super method and writes the MBR object of this entry to the
         * specified output stream.
         * 
         * @param out the stream to write the object to
         * @throws java.io.IOException Includes any I/O exceptions that may occur
         */

        //public void writeExternal(ObjectOutput out) throws IOException {
        //  super.writeExternal(out);
        //  mbr.writeExternal(out);
        //}

        ///**
        // * Calls the super method and reads the MBR object of this entry from the
        // * specified input stream.
        // * 
        // * @param in the stream to read data from in order to restore the object
        // * @throws java.io.IOException if I/O errors occur
        // * @throws ClassNotFoundException If the class for an object being restored
        // *         cannot be found.
        // */

        //public void readExternal(ObjectInput in) throws IOException, ClassNotFoundException {
        //  super.readExternal(in);
        //  this.mbr = new ModifiableHyperBoundingBox();
        //  this.mbr.readExternal(in);
        //}

        /**
         * Extend the MBR of this node.
         * 
         * @param responsibleMBR
         * @return true when the MBR changed
         */
        public bool ExtendMBR(ISpatialComparable responsibleMBR)
        {
            return this.mbr.extend(responsibleMBR);
        }
    }
}
