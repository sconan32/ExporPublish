using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Socona.Expor.Data;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Indexes.Tree.Spatial
{

    public class SpatialPointLeafEntry : AbstractLeafEntry, ISpatialEntry
    {
        //private static long serialVersionUID = 1;

        /**
         * The values of the underlying data object.
         */
        private double[] values;

        /**
         * Empty constructor for serialization purposes.
         */
        public SpatialPointLeafEntry() :
            base()
        {
        }

        /**
         * Constructs a new LeafEntry object with the given parameters.
         * 
         * @param id the unique id of the underlying data object
         * @param values the values of the underlying data object
         */
        public SpatialPointLeafEntry(IDbId id, double[] values) :
            base(id)
        {
            this.values = values;
        }

        /**
         * Constructor from number vector
         *
         * @param id Object id
         * @param vector Number vector
         */
        public SpatialPointLeafEntry(IDbId id, INumberVector vector) :
            base(id)
        {
            int dim = vector.Count;
            this.values = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                values[i] = vector[(i + 1)];
            }
        }


        public int Count
        {
            get { return values.Length; }
        }

        /**
         * @return the value at the specified dimension
         */

        public double GetMin(int dimension)
        {
            return values[dimension - 1];
        }

        /**
         * @return the value at the specified dimension
         */

        public double GetMax(int dimension)
        {
            return values[dimension - 1];
        }

        /**
         * Returns the values of the underlying data object of this entry.
         * 
         * @return the values of the underlying data object of this entry
         */
        public double[] GetValues()
        {
            return values;
        }

        /**
         * Calls the base method and writes the values of this entry to the specified
         * stream.
         * 
         * @param out the stream to write the object to
         * @throws java.io.IOException Includes any I/O exceptions that may occur
         */

        //public void WriteExternal(MemoryStream sout)
        //{
        //    base.writeExternal(sout);
        //    sout.writeInt(values.Length);
        //    foreach (double v in values)
        //    {
        //        sout.writeDouble(v);
        //    }
        //}

        ///**
        // * Calls the base method and reads the values of this entry from the
        // * specified input stream.
        // * 
        // * @param in the stream to read data from in order to restore the object
        // * @throws java.io.IOException if I/O errors occur
        // * @throws ClassNotFoundException If the class for an object being restored
        // *         cannot be found.
        // */

        //public void readExternal(ObjectInput sin)
        //{
        //    base.readExternal(sin);
        //    values = new double[sin.readInt()];
        //    for (int d = 0; d < values.Length; d++)
        //    {
        //        values[d] = sin.readDouble();
        //    }
        //}
    }
}
