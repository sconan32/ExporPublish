using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Socona.Expor.Data.Spatial;
using Socona.Expor.Utilities;

namespace Socona.Expor.Data
{

    public class HyperBoundingBox : ISpatialComparable, ISerializable
    {
        /**
         * Serial version
         */
        //private static long serialVersionUID = 1;

        /**
         * The coordinates of the 'lower left' (= minimum) hyper point.
         */
        protected double[] min;

        /**
         * The coordinates of the 'upper right' (= maximum) hyper point.
         */
        protected double[] max;

        /**
         * Empty constructor for Externalizable interface.
         */
        public HyperBoundingBox()
        {
            // nothing to do
        }

        /**
         * Creates a HyperBoundingBox for the given hyper points.
         * 
         * @param min - the coordinates of the minimum hyper point
         * @param max - the coordinates of the maximum hyper point
         */
        public HyperBoundingBox(double[] min, double[] max)
        {
            if (min.Length != max.Length)
            {
                throw new ArgumentException("min/max need same dimensionality");
            }

            this.min = min;
            this.max = max;
        }

        /**
         * Constructor, cloning an existing spatial object.
         * 
         * @param other Object to clone
         */
        public HyperBoundingBox(ISpatialComparable other)
        {
            int dim = other.Count;
            this.min = new double[dim];
            this.max = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                this.min[i] = other.GetMin(i + 1);
                this.max[i] = other.GetMax(i + 1);
            }
        }

        /**
         * Returns the coordinate at the specified dimension of the minimum hyper
         * point
         * 
         * @param dimension the dimension for which the coordinate should be returned,
         *        where 1 &le; dimension &le; <code>this.GetDimensionality()</code>
         * @return the coordinate at the specified dimension of the minimum hyper
         *         point
         */

        public double GetMin(int dimension)
        {
            return min[dimension - 1];
        }

        /**
         * Returns the coordinate at the specified dimension of the maximum hyper
         * point
         * 
         * @param dimension the dimension for which the coordinate should be returned,
         *        where 1 &le; dimension &le; <code>this.GetDimensionality()</code>
         * @return the coordinate at the specified dimension of the maximum hyper
         *         point
         */

        public double GetMax(int dimension)
        {
            return max[dimension - 1];
        }

        /**
         * Returns the dimensionality of this HyperBoundingBox.
         * 
         * @return the dimensionality of this HyperBoundingBox
         */

        public int Count
        {
            get { return min.Length; }
        }

        /**
         * Returns a String representation of the HyperBoundingBox.
         * 
         * @return a string representation of this hyper bounding box
         */

        public override String ToString()
        {
            return "[Min(" + FormatUtil.Format(min, ",", 10) + "), Max(" + FormatUtil.Format(max, ",", 10) + ")]";
        }

        /**
         * Returns a String representation of the HyperBoundingBox.
         * 
         * @param nf number Format for output accuracy
         * @param pre the prefix of each line
         * @return a string representation of this hyper bounding box
         */
        public String ToString(String pre, NumberFormatInfo nf)
        {
            return pre + "[Min(" + FormatUtil.Format(min, ",", nf) + "), Max(" + FormatUtil.Format(max, ",", nf) + ")]";
        }

        /**
         * @see Object#equals(Object)
         */

        public override bool Equals(Object obj)
        {
            HyperBoundingBox box = (HyperBoundingBox)obj;
            return Array.Equals(min, box.min) && Array.Equals(max, box.max);
        }

        /**
         * @see Object#hashCode()
         */

        public override int GetHashCode()
        {
            return 29 * min.GetHashCode() + max.GetHashCode();
        }

        /**
         * The object implements the writeExternal method to save its contents by
         * calling the methods of DataOutput for its primitive values or calling the
         * writeObject method of ObjectOutput for objects, strings, and arrays.
         * 
         * @param out the stream to write the object to
         * @throws java.io.IOException Includes any I/O exceptions that may occur
         * @serialData Overriding methods should use this tag to describe the data
         *             layout of this Externalizable object. List the sequence of
         *             element types and, if possible, relate the element to a
         *             public/protected field and/or method of this Externalizable
         *             class.
         */

        //public void writeExternal(ObjectOutput out) throws IOException {
        //  int dim = GetDimensionality();
        //  out.writeInt(dim);

        //  for(double aMin : min) {
        //    out.writeDouble(aMin);
        //  }

        //  for(double aMax : max) {
        //    out.writeDouble(aMax);
        //  }
        //}

        /**
         * The object implements the readExternal method to restore its contents by
         * calling the methods of DataInput for primitive types and readObject for
         * objects, strings and arrays. The readExternal method must read the values
         * in the same sequence and with the same types as were written by
         * writeExternal.
         * 
         * @param in the stream to read data from in order to restore the object
         * @throws java.io.IOException if I/O errors occur
         * @throws ClassNotFoundException If the class for an object being restored
         *         cannot be found.
         */

        //public void readExternal(ObjectInput in) throws IOException, ClassNotFoundException {
        //  int dim = in.readInt();
        //  min = new double[dim];
        //  max = new double[dim];

        //  for(int i = 0; i < min.Length; i++) {
        //    min[i] = in.readDouble();
        //  }

        //  for(int i = 0; i < max.Length; i++) {
        //    max[i] = in.readDouble();
        //  }
        //}

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
