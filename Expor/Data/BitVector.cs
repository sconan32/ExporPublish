using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using System.Collections;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.DataStructures;
using Socona.Expor.Utilities.Options;
using System.Diagnostics;
using System.IO;

namespace Socona.Expor.Data
{

    public class BitVector : AbstractNumberVector<Bit>, IByteBufferSerializer
    {
        /**
         * Static instance.
         */
        public static readonly BitVector STATIC = new BitVector(new BitArray(0), 0);
        //public readonly static String ATTRIBUTE_SEPARATOR = " ";
        /**
         * Storing the bits.
         */
        private BitArray bits;

        /**
         * Dimensionality of this bit vector.
         */
        private int dimensionality;

        /**
         * Provides a new BitVector corresponding to the specified bits and of the
         * specified dimensionality.
         * 
         * @param bits the bits to be set in this BitVector
         * @param dimensionality the dimensionality of this BitVector
         * @throws IllegalArgumentException if the specified dimensionality is to
         *         small to match the given BitSet
         */
        public BitVector(BitArray bits, int dimensionality)
        {
            if (dimensionality < bits.Length)
            {
                throw new ArgumentException("Specified dimensionality " + dimensionality + " is to low for specified BitSet of length " + bits.Length);
            }
            this.bits = bits;
            this.dimensionality = dimensionality;
        }

        /**
         * Provides a new BitVector corresponding to the bits in the given array.
         * 
         * @param bits an array of bits specifying the bits in this bit vector
         */
        public BitVector(Bit[] bits)
        {
            this.bits = new BitArray(bits.Length);
            for (int i = 0; i < bits.Length; i++)
            {
                this.bits[i] = Convert.ToBoolean(bits[i]);
            }
            this.dimensionality = bits.Length;
        }
        public BitVector(int dimensionality)
        {
            bits = new BitArray(dimensionality);
            this.dimensionality = dimensionality;
        }
        /**
         * The dimensionality of the binary vector space of which this BitVector is an
         * element.
         * 
         * @see de.lmu.ifi.dbs.elki.data.NumberVector#getDimensionality()
         */

        public override int Count
        {
            get
            {
                return dimensionality;
            }
        }

        /**
         * Returns the value in the specified dimension.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         * 
         * @see de.lmu.ifi.dbs.elki.data.NumberVector#getValue(int)
         */

        public Bit GetValue(int dimension)
        {
            if (dimension < 1 || dimension > dimensionality)
            {
                throw new ArgumentException("illegal dimension: " + dimension);
            }
            return new Bit(bits[dimension - 1]);
        }

        /**
         * Returns the value in the specified dimension as double.
         * 
         * @param dimension the desired dimension, where 1 &le; dimension &le;
         *        <code>this.getDimensionality()</code>
         * @return the value in the specified dimension
         * 
         * @see de.lmu.ifi.dbs.elki.data.NumberVector#doubleValue(int)
         */

        public double DoubleValue(int dimension)
        {
            if (dimension < 1 || dimension > dimensionality)
            {
                throw new ArgumentException("illegal dimension: " + dimension);
            }
            return bits[dimension - 1] ? 1.0 : 0.0;
        }

        ///**
        // * Returns the value in the specified dimension as long.
        // * 
        // * @param dimension the desired dimension, where 1 &le; dimension &le;
        // *        <code>this.getDimensionality()</code>
        // * @return the value in the specified dimension
        // * 
        // * @see de.lmu.ifi.dbs.elki.data.NumberVector#longValue(int)
        // */

        //public long longValue(int dimension) {
        //  if(dimension < 1 || dimension > dimensionality) {
        //    throw new IllegalArgumentException("illegal dimension: " + dimension);
        //  }
        //  return bits.get(dimension - 1) ? 1 : 0;
        //}

        /**
         * Returns a Vector representing in one column and
         * <code>getDimensionality()</code> rows the values of this BitVector as
         * double values.
         * 
         * @return a Matrix representing in one column and
         *         <code>getDimensionality()</code> rows the values of this BitVector
         *         as double values
         * 
         * @see de.lmu.ifi.dbs.elki.data.NumberVector#getColumnVector()
         */

        public override Vector GetColumnVector()
        {
            double[] values = new double[dimensionality];
            for (int i = 0; i < dimensionality; i++)
            {
                values[i] = bits[i] ? 1 : 0;
            }
            return new Vector(values);
        }

        /**
         * Returns whether this BitVector contains all bits that are set to true in
         * the specified BitSet.
         * 
         * @param bitset the bits to inspect in this BitVector
         * @return true if this BitVector contains all bits that are set to true in
         *         the specified BitSet, false otherwise
         */
        public bool Contains(BitArray bitset)
        {
            bool contains = true;
            for (int i = 0; i < bitset.Length; i++)
            {
                // noinspection ConstantConditions
                if (bits[i] == true)
                {
                    contains &= bits[i];
                }
            }
            return contains;
        }

        /**
         * Returns a copy of the bits currently set in this BitVector.
         * 
         * @return a copy of the bits currently set in this BitVector
         */
        public BitArray GetBits()
        {
            return (BitArray)bits.Clone();
        }

        /**
         * Returns a String representation of this BitVector. The representation is
         * suitable to be parsed by
         * {@link de.lmu.ifi.dbs.elki.datasource.parser.BitVectorLabelParser
         * BitVectorLabelParser}.
         * 
         * @see Object#toString()
         */

        public override String ToString()
        {
            Bit[] bitArray = new Bit[dimensionality];
            for (int i = 0; i < dimensionality; i++)
            {
                bitArray[i] = new Bit(bits[i]);
            }
            StringBuilder representation = new StringBuilder();
            foreach (Bit bit in bitArray)
            {
                if (representation.Length > 0)
                {
                    representation.Append(ATTRIBUTE_SEPARATOR);
                }
                representation.Append(bit.ToString());
            }
            return representation.ToString();
        }

        /**
         * Indicates whether some other object is "equal to" this BitVector. This
         * BitVector is equal to the given object, if the object is a BitVector of
         * same dimensionality and with identical bits set.
         */

        public override bool Equals(Object obj)
        {
            if (obj is BitVector)
            {
                BitVector bv = (BitVector)obj;
                return this.Count == bv.Count && this.bits.Equals(bv.bits);

            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {

            return base.GetHashCode();
        }


        public IDataVector NewFeatureVector(IList<Bit> array, IArrayAdapter adapter)
        {
            int dim = adapter.Size(array);
            BitArray bits = new BitArray(dim);
            for (int i = 0; i < dim; i++)
            {
                bits[i] = Convert.ToBoolean(adapter.Get(array, i));
                i++;
            }
            return new BitVector(bits, dim);
        }


        public IDataVector NewNumberVector(IList<Bit> array, IArrayAdapter adapter)
        {
            int dim = adapter.Size(array);
            BitArray bits = new BitArray(dim);
            for (int i = 0; i < dim; i++)
            {
                if ((double)adapter.Get(array, i) >= 0.5)
                {
                    bits.Set(i, true);
                }
            }
            return new BitVector(bits, dim);
        }


        public BitVector FromByteBuffer(ByteBuffer buffer)
        {
            short dimensionality = buffer.GetInt16();
            int len = ByteArrayUtil.SIZE_SHORT + (dimensionality + 7) / 8;
            if (buffer.Remaining < len)
            {
                throw new IOException("Not enough data for a bit vector!");
            }
            // read values
            BitArray values = new BitArray(dimensionality);
            byte b = 0;
            for (int i = 0; i < dimensionality; i++)
            {
                // read the next byte when needed.
                if ((i & 7) == 0)
                {
                    b = buffer.GetByte();
                }
                byte bit = (byte)(1 << (i & 7));
                if ((b & bit) != 0)
                {
                    values.Set(i + 1, true);
                }
            }
            return new BitVector(values, dimensionality);
        }


        public void ToByteBuffer(ByteBuffer buffer, BitVector vec)
        {
            int len = GetByteSize(vec);
            Debug.Assert(vec.Count <= Int16.MaxValue);
            short dim = (short)vec.Count;
            if (buffer.Remaining < len)
            {
                throw new IOException("Not enough space for the bit vector!");
            }
            // write size
            buffer.Write(dim);
            // write values
            // Next byte to write:
            byte b = 0;
            for (int i = 0; i < dim; i++)
            {
                byte mask = (byte)(1 << (i & 7));
                if (vec.bits[i])
                {
                    b |= mask;
                }
                else
                {
                    b &= (byte)~mask;
                }
                // Write when appropriate
                if ((i & 7) == 7 || i == dim - 1)
                {
                    buffer.Write(b);
                    b = 0;
                }
            }
        }


        public int GetByteSize(BitVector vec)
        {
            return ByteArrayUtil.SIZE_SHORT + (vec.Count + 7) / 8;
        }

        /**
         * Parameterization class
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        public class Parameterizer : AbstractParameterizer
        {

            protected override object MakeInstance()
            {
                return STATIC;
            }


        }

        public object FromByteBuffer(Type type, ByteBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
        {
            throw new NotImplementedException();
        }

        public int GetByteSize(object o, Type type)
        {
            throw new NotImplementedException();
        }

        public override Bit this[int ind]
        {
            get { return bits[ind]; }
            set { this.bits[ind] = value; }
        }


        public override INumberVector NewNumberVector(double[] values)
        {
            throw new NotImplementedException();
        }

        public override IDataVector NewFeatureVector(IList<object> array, IArrayAdapter adapter)
        {
            throw new NotImplementedException();
        }
    }
}
