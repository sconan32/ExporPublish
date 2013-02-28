using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.DataStructures;
using Socona.Expor.Utilities.Options;
using System.IO;
using Socona.Expor.Utilities;

namespace Socona.Expor.Data
{

    /// <summary>
    /// A DoubleVector is to store real values approximately as double values.
    /// <para>NOTICE: The vector value will be copied and stored </para>
    /// </summary>
    public class DoubleVector : AbstractNumberVector<double>, IByteBufferSerializer
    {
        /**
         * Static factory instance
         */
        public static readonly DoubleVector STATIC = new DoubleVector(new double[0], true);

        /**
         * Keeps the values of the real vector
         */
        private double[] values;

        /**
         * Private constructor. NOT for public use.
         */
        private DoubleVector(double[] values, bool nocopy)
        {
            if (nocopy)
            {
                this.values = values;
            }
            else
            {
                this.values = new double[values.Length];
                Array.Copy(values, 0, this.values, 0, values.Length);
            }
        }

        /**
         * Provides a feature vector consisting of double values according to the
         * given Double values.
         * 
         * @param values the values to be set as values of the real vector
         */
        public DoubleVector(List<Double> values)
        {
            int i = 0;
            this.values = new double[values.Count()];
            var it = values.GetEnumerator();
            do
            {
                values[i++] = it.Current;
            } while (it.MoveNext());
        }

        /**
         * Provides a DoubleVector consisting of the given double values.
         * 
         * @param values the values to be set as values of the DoubleVector
         */
        public DoubleVector(double[] values)
        {
            this.values = new double[values.Length];
            Array.Copy(values, 0, this.values, 0, values.Length);
        }

        ///**
        // * Provides a DoubleVector consisting of the given double values.
        // * 
        // * @param values the values to be set as values of the DoubleVector
        // */
        //public DoubleVector(Double[] values) {
        //  this.values = new double[values.Length];
        //  for(int i = 0; i < values.Length; i++) {
        //    this.values[i] = values[i];
        //  }
        //}

        /**
         * Expects a matrix of one column.
         * 
         * @param columnMatrix a matrix of one column
         */
        public DoubleVector(Vector columnMatrix)
        {
            values = new double[columnMatrix.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = (double)columnMatrix.Get(i);
            }
        }


        public int GetDimensionality()
        {
            return values.Length;
        }

        /**
         * Returns the value of the specified attribute.
         * 
         * @param dimension the selected attribute. Attributes are counted starting
         *        with 1.
         * 
         * @throws IllegalArgumentException if the specified dimension is out of range
         *         of the possible attributes
         */

        public Double GetValue(int dimension)
        {
            try
            {
                return values[dimension - 1];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("Dimension " + dimension + " out of range.");
            }
        }

        /**
         * Returns the value of the specified attribute.
         * 
         * @param dimension the selected attribute. Attributes are counted starting
         *        with 1.
         * 
         * @throws IllegalArgumentException if the specified dimension is out of range
         *         of the possible attributes
         */

        public double DoubleValue(int dimension)
        {
            try
            {
                return values[dimension - 1];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("Dimension " + dimension + " out of range.");
            }
        }

        /**
         * Returns the value of the specified attribute as long.
         * 
         * @param dimension the selected attribute. Attributes are counted starting
         *        with 1.
         * 
         * @throws IllegalArgumentException if the specified dimension is out of range
         *         of the possible attributes
         */

        public long LongValue(int dimension)
        {
            try
            {
                return (long)values[dimension - 1];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("Dimension " + dimension + " out of range.");
            }
        }

        /**
         * Get a copy of the raw double[] array.
         * 
         * @return copy of values array.
         */
        public double[] GetValues()
        {
            double[] copy = new double[values.Length];
            Array.Copy(values, 0, copy, 0, values.Length);
            return copy;
        }


        public override Vector GetColumnVector()
        {
            // TODO: can we sometimes save this copy?
            // Is this worth the more complex API?
            return new Vector((double[])values.Clone());
        }

        public override String ToString()
        {
            StringBuilder featureLine = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                featureLine.Append(FormatUtil.Format(values[i]));
                if (i + 1 < values.Length)
                {
                    featureLine.Append(ATTRIBUTE_SEPARATOR);
                }
            }
            return featureLine.ToString();
        }


        public override INumberVector NewNumberVector(double[] values)
        {
            return new DoubleVector(values);
        }


        public override IDataVector NewFeatureVector(IList<object> array, IArrayAdapter adapter)
        {
            int dim = adapter.Size(array);
            double[] values = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                values[i] = (double)adapter.Get(array, i);
            }
            return new DoubleVector(values, true);
        }


        //public DoubleVector NewNumberVector<A>(A array, INumberArrayAdapter<ValueType, A> adapter) {
        //  if(adapter == ArrayLikeUtil.TDOUBLELISTADAPTER) {
        //    return new DoubleVector(((TDoubleList) array).toArray(), true);
        //  }
        //   int dim = adapter.Size(array);
        //  double[] values = new double[dim];
        //  for(int i = 0; i < dim; i++) {
        //    values[i] = adapter.GetDouble(array, i);
        //  }
        //  return new DoubleVector(values, true);
        //}


        public DoubleVector FromByteBuffer(ByteBuffer buffer)
        {
            short dimensionality = buffer.GetInt16();
            int len = ByteArrayUtil.SIZE_SHORT + ByteArrayUtil.SIZE_DOUBLE * dimensionality;
            if (buffer.Remaining < len)
            {
                throw new IOException("Not enough data for a double vector!");
            }
            double[] values = new double[dimensionality];
            buffer.GetDoubles(values);
            return new DoubleVector(values, true);
        }


        public void ToByteBuffer(ByteBuffer buffer, DoubleVector vec)
        {
            short dimensionality = buffer.GetInt16();
            int len = ByteArrayUtil.SIZE_SHORT + ByteArrayUtil.SIZE_DOUBLE * dimensionality;
            if (buffer.Remaining < len)
            {
                throw new IOException("Not enough space for the double vector!");
            }
            buffer.Write(dimensionality);
            buffer.Write(vec.values);
        }


        public int GetByteSize(DoubleVector vec)
        {
            return ByteArrayUtil.SIZE_SHORT + ByteArrayUtil.SIZE_DOUBLE * vec.GetDimensionality();
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

        public override int Count
        {
            get { return values.Length; }
        }

        public override double this[int ind]
        {
            get { return values[ind]; }
            set { values[ind] = value; }
        }
    }
}
