using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using Socona.Expor.Data;
using Socona.Expor.Utilities;

namespace Socona.Expor.Maths.LinearAlgebra
{
    public class Vector : MathNet.Numerics.LinearAlgebra.Double.DenseVector, INumberVector
    {

        public static implicit operator Vector(double[] vec)
        {
            return new Vector(vec);
        }
        public Vector(double[] d)
            : base(d)
        {

        }
        public Vector(int size)
            : base(size)
        {
        }

        public Vector(Vector v)
            : base(v.ToArray())
        {
        }
        public double Get(int i)
        {
            return base[i];
        }
        static public Vector operator *(Vector v, double d)
        {
            Vector res = new Vector(v);
            res.DoMultiply(d, res);
            return res;
        }
        public static Vector operator +(Vector v1, Vector v2)
        {
            Vector res = new Vector(v1);
            res.DoAdd(v2, res);
            return res;

        }
        static public Vector operator /(Vector v, double d)
        {
            Vector res = new Vector(v);
            res.DoDivide(d, res);
            return res;
        }
        public static Vector operator -(Vector v1, Vector v2)
        {
            Vector res = new Vector(v1);
            res.DoSubtract(v2, res);
            return res;
        }
        // public int Count { get { return base.Count; } }
        public int Length { get { return base.Count; } }
        /// <summary>
        /// (this^T)*B
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public Matrix TransposeTimes(Matrix B)
        {
            Debug.Assert(B.RowCount == this.Count, "Matrix inner dimensions must agree.");
            Matrix X = new Matrix(1, B.ColumnCount);
            for (int j = 0; j < B.ColumnCount; j++)
            {
                // multiply it with each row from A
                double s = 0;
                for (int k = 0; k < this.Count; k++)
                {
                    s += this[k] * B[k, j];
                }
                X[0, j] = s;
            }
            return X;
        }
        /// <summary>
        /// (this^T)*B
        /// </summary>
        /// <param name="Vector"></param>
        /// <returns></returns>
        public double TransposeTimes(Vector B)
        {
            Debug.Assert(B.Count == this.Count, "Matrix inner dimensions must agree.");
            double s = 0;
            for (int k = 0; k < this.Count; k++)
            {
                s += this[k] * B[k];
            }
            return s;
        }
        /// <summary>
        /// (this^T)*B*c
        /// </summary>
        /// <param name="B"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public double TransposeTimesTimes(Matrix B, Vector c)
        {
            Debug.Assert(B.RowCount == this.Count, "Matrix inner dimensions must agree.");
            double sum = 0.0;
            for (int j = 0; j < B.ColumnCount; j++)
            {
                // multiply it with each row from A
                double s = 0;
                for (int k = 0; k < this.Count; k++)
                {
                    s += this[k] * B[k, j];
                }
                sum += s * c[j];
            }
            return sum;
        }
        public Vector PlusEquals(double d)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i] += d;
            }
            return this;
        }
        /**
 * A = A + B
 * 
 * @param B another matrix
 * @return A + B in this Matrix
 */
        public Vector PlusEquals(Vector B)
        {
            Debug.Assert(this.Count == B.Count, "Vector dimensions must agree.");
            for (int i = 0; i < this.Count; i++)
            {
                this[i] += B[i];
            }
            return this;
        }
        public Vector PlusTimesEquals(Vector B, double s)
        {
            Debug.Assert(this.Count == base.Count, "Vector dimensions must agree.");
            for (int i = 0; i < this.Count; i++)
            {
                this[i] += s * B[i];
            }
            return this;
        }
        public Vector MinusEquals(double d)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i] -= d;

            }
            return this;
        }
        public Vector MinusEquals(Vector B)
        {
            Debug.Assert(this.Count == B.Count, "Vector dimensions must agree.");
            for (int i = 0; i < this.Count; i++)
            {
                this[i] -= B[i];
            }
            return this;
        }


        /**
         * A = A - s * B
         * 
         * @param B another matrix
         * @param s Scalar
         * @return A - s * B in this Matrix
         */
        public Vector MinusTimesEquals(Vector B, double s)
        {
            Debug.Assert(this.Count == B.Count, "Vector dimensions must agree.");
            for (int i = 0; i < this.Count; i++)
            {
                this[i] -= s * B[i];
            }
            return this;
        }
        public Vector TimesEquals(double s)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i] *= s;
            }
            return this;
        }
        /// <summary>
        /// A * B^T
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public Matrix TimesTranspose(Vector B)
        {
            Matrix X = new Matrix(this.Count, B.Count);
            for (int j = 0; j < B.Count; j++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    X[i, j] = this[i] * B[j];
                }
            }
            return X;

        }

        public double[] GetArrayRef()
        {
            return base.ToArray();
        }
        public Vector GetColumnVector()
        {
            return new Vector(base.Clone().ToArray());
        }

        public INumberVector NewNumberVector(double[] values)
        {
            throw new NotImplementedException();
        }


        public IDataVector NewFeatureVector(IList<object> array, Utilities.DataStructures.ArrayLike.IArrayAdapter adapter)
        {
            throw new NotImplementedException();
        }

        public Vector Projection(Matrix v)
        {
            Debug.Assert(this.Length == v.RowCount, "p and v differ in row dimensionality!");
            Vector sum = new Vector(Length);
            for (int i = 0; i < v.ColumnCount; i++)
            {
                // TODO: optimize - copy less?
                Vector v_i = v.Column(i);
                sum.PlusTimesEquals(v_i, this.TransposeTimes(v_i));
            }
            return sum;
        }

        /**
         * Returns the length of this vector.
         * 
         * @return the length of this vector
         */
        public double EuclideanLength()
        {
            double acc = 0.0;
            for (int row = 0; row < this.Length; row++)
            {
                double v = this[row];
                acc += v * v;
            }
            return Math.Sqrt(acc);
        }

        /**
         * Normalizes this vector to the length of 1.0.
         */
        public Vector Normalize()
        {
            double norm = EuclideanLength();
            if (norm != 0)
            {
                for (int row = 0; row < this.Length; row++)
                {
                    this[row] /= norm;
                }
            }
            return this;
        }

        public double GetMin(int dimension)
        {
            throw new NotImplementedException();
        }

        public double GetMax(int dimension)
        {
            throw new NotImplementedException();
        }

        /**
         * Returns a string representation of this vector without adding extra
         * whitespace
         * 
         * @return a string representation of this vector.
         */
        public String ToStringNoWhitespace()
        {
            return "[" + FormatUtil.Format(this.ToArray(), ",") + "]";
        }
        object IDataVector.Get(int dim)
        {
            return this[dim];
        }
    }
}
