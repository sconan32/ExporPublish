using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Persistent;
using Socona.Expor.Maths.LinearAlgebra;
using Socona.Expor.Utilities.DataStructures.ArrayLike;
using Socona.Expor.Utilities.DataStructures;
using System.IO;
using Socona.Expor.Utilities.Options;


namespace Socona.Expor.Data
{

    public class FloatVector :AbstractNumberVector<float>, IByteBufferSerializer
    {
        /**
         * Static factory instance
         */
        public static readonly FloatVector STATIC = new FloatVector(new float[0], true);
        //public readonly static String ATTRIBUTE_SEPARATOR = " ";

        /**
         * Keeps the values of the float vector
         */
        private float[] values;

        /**
         * Private constructor. NOT for public use.
         */
        private FloatVector(float[] values, bool nocopy)
        {
            if (nocopy)
            {
                this.values = values;
            }
            else
            {
                this.values = new float[values.Length];
                Array.Copy(values, 0, this.values, 0, values.Length);
            }
        }

        /**
         * Provides a FloatVector consisting of float values according to the given
         * Float values.
         * 
         * @param values the values to be set as values of the float vector
         */
        public FloatVector(List<float> values)
        {
            int i = 0;
            this.values = new float[values.Count()];
            var it = values.GetEnumerator();
            do
            {
                values[i++] = it.Current;
            } while (it.MoveNext());
        }


        /**
         * Provides a FloatVector consisting of the given float values.
         * 
         * @param values the values to be set as values of the float vector
         */
        public FloatVector(float[] values)
        {
            this.values = new float[values.Length];
            Array.Copy(values, 0, this.values, 0, values.Length);
        }

        ///**
        // * Provides a FloatVector consisting of the given float values.
        // * 
        // * @param values the values to be set as values of the float vector
        // */
        //public FloatVector(float[] values) {
        //  this.values = new float[values.Length];
        //  for(int i = 0; i < values.Length; i++) {
        //    this.values[i] = values[i];
        //  }
        //}

        /**
         * Expects a matrix of one column.
         * 
         * @param columnMatrix a matrix of one column
         */
        public FloatVector(Vector columnMatrix)
        {
            values = new float[columnMatrix.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = (float)columnMatrix.Get(i);
            }
        }


        public int GetDimensionality()
        {
            return values.Length;
        }


        public float GetValue(int dimension)
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


        public override  Vector GetColumnVector()
        {
            return new Vector(ArrayLikeUtil.ToPrimitiveDoubleArray(values, ArrayLikeUtil.FLOATARRAYADAPTER));
        }


        public override String ToString()
        {
            StringBuilder featureLine = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                featureLine.Append(values[i]);
                if (i + 1 < values.Length)
                {
                    featureLine.Append(ATTRIBUTE_SEPARATOR);
                }
            }
            return featureLine.ToString();
        }


        public override  IDataVector NewFeatureVector(IList<object> array, IArrayAdapter adapter)
        {
            int dim = adapter.Size(array);
            float[] values = new float[dim];
            for (int i = 0; i < dim; i++)
            {
                values[i] = (float)adapter.Get(array, i);
            }
            return new FloatVector(values, true);
        }


        //public override FloatVector NewNumberVector<A>(A array, INumberArrayAdapter<ValueType, A> adapter) {
        //  int dim = adapter.Size(array);
        //  float[] values = new float[dim];
        //  for(int i = 0; i < dim; i++) {
        //    values[i] = adapter.GetFloat(array, i);
        //  }
        //  return new FloatVector(values, true);
        //}


        public FloatVector FromByteBuffer(ByteBuffer buffer)
        {
            short dimensionality = buffer.GetInt16();
            int len = ByteArrayUtil.SIZE_SHORT + ByteArrayUtil.SIZE_FLOAT * dimensionality;
            if (buffer.Remaining < len)
            {
                throw new IOException("Not enough data for a float vector!");
            }
            // read the values
            float[] values = new float[dimensionality];
            buffer.GetSingles(values);
            return new FloatVector(values, false);
        }


        public void ToByteBuffer(ByteBuffer buffer, FloatVector vec)
        {
            short dimensionality = buffer.GetInt16();
            int len = GetByteSize(vec);
            if (buffer.Remaining < len)
            {
                throw new IOException("Not enough space for the float vector!");
            }
            // write dimensionality
            buffer.Write(dimensionality);
            buffer.Write(vec.values);
        }


        public int GetByteSize(FloatVector vec)
        {
            return ByteArrayUtil.SIZE_SHORT + ByteArrayUtil.SIZE_FLOAT * vec.GetDimensionality();
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





        public override float this[int dimension]
        {
            get { return values[dimension]; }
            set { values[dimension] = value; }
        }

        public override int Count
        {
            get { return values.Count(); }
        }

        public override INumberVector NewNumberVector(double[] values)
        {
            throw new NotImplementedException();
        }
    }
}
