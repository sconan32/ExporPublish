using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;
using Socona.Log;

namespace Socona.Expor.Maths.LinearAlgebra
{
    public class Matrix : MathNet.Numerics.LinearAlgebra.Double.DenseMatrix
    {
        public static double DELTA = 1E-3;

        public Matrix(double[,] array)
            : base(array)
        {
        }
        public Matrix(int order)
            : base(order)
        {
        }
        public Matrix(Matrix m)
            : base(m.ToArray())
        {

        }
        public Matrix(int rows, int columns) : base(rows, columns) { }


        public Matrix(double[] values, int m)
            : base(values.Length / m, m)
        {
            int ColumnCount = (m != 0 ? values.Length / m : 0);
            //if(m * ColumnCount != values.length) {
            //  throw new IllegalArgumentException("Array length must be a multiple of m.");
            //}
            //elements = new double[m][ColumnCount];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    this[i, j] = values[i + j * m];
                }
            }
        }
        public Matrix(int rows, int columns, double value)
            : base(rows, columns, value)
        { }
        public static Matrix operator +(Matrix left, Matrix right)
        {
            Matrix res = new Matrix(left.RowCount, left.ColumnCount);
            left.DoAdd(right, res);
            return res;
        }
        public static Matrix operator *(Matrix left, Matrix right)
        {
            Matrix res = null;
            left.DoMultiply(right, res);
            return res;
        }
        public static Matrix operator *(Matrix left, double right)
        {
            Matrix res = new Matrix(left.RowCount, left.ColumnCount);
            left.DoMultiply(right, res);
            return res;
        }
        public static Matrix UnitMatrix(int dim)
        {
            double[,] e = new double[dim, dim];
            for (int i = 0; i < dim; i++)
            {
                e[i, i] = 1;
            }
            return new Matrix(e);
        }
        public Matrix CheatToAvoidSingularity(double constant)
        {
            Matrix a = (Matrix)this.Clone();

            for (int i = 0; i < a.ColumnCount && i < a.RowCount; i++)
            {
                a[i, i] += constant;
            }
            return a;
        }
        public new Vector Column(int i)
        {
            return (Vector)base.Column(i).ToArray();
        }

        public new Vector Row(int i)
        {
            return (Vector)base.Row(i).ToArray();
        }


        /**
         * Returns the zero matrix of the specified dimension.
         * 
         * @param dim the dimensionality of the unit matrix
         * @return the zero matrix of the specified dimension
         */
        public static Matrix ZeroMatrix(int dim)
        {
            double[,] z = new double[dim, dim];
            return new Matrix(z);
        }

        /**
         * Generate matrix with random elements
         * 
         * @param m Number of rows.
         * @param n Number of columns.
         * @return An m-by-n matrix with uniformly distributed random elements.
         */
        public static Matrix Random(int m, int n)
        {
            Matrix A = new Matrix(m, n);
            Random ran = new Random();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = ran.NextDouble();
                }
            }
            return A;
        }

        public static Matrix Identity(int m, int n)
        {
            Matrix A = new Matrix(m, n);
            for (int i = 0; i < Math.Min(m, n); i++)
            {
                A[i, i] = 1.0;
            }
            return A;
        }

        /**
         * Returns a quadratic Matrix consisting of zeros and of the given values on
         * the diagonal.
         * 
         * @param diagonal the values on the diagonal
         * @return the resulting matrix
         */
        public static Matrix Diagonal(double[] diagonal)
        {
            Matrix result = new Matrix(diagonal.Length, diagonal.Length);
            for (int i = 0; i < diagonal.Length; i++)
            {
                result[i, i] = diagonal[i];
            }
            return result;
        }

        /**
         * Returns a quadratic Matrix consisting of zeros and of the given values on
         * the diagonal.
         * 
         * @param diagonal the values on the diagonal
         * @return the resulting matrix
         */
        public static Matrix Diagonal(Vector diagonal)
        {
            Matrix result = new Matrix(diagonal.Length, diagonal.Length);
            for (int i = 0; i < diagonal.Length; i++)
            {
                result[i, i] = diagonal[i];
            }
            return result;
        }
        public new object Clone()
        {
            return new Matrix(this);
        }
        public new Matrix Inverse()
        {

            Matrix matrix = new Matrix(base.Inverse().ToArray());
            return matrix;
        }
        public double[,] GetArrayRef()
        {
            return base.ToArray();
        }

        /**
         * Copy the internal two-dimensional array.
         * 
         * @return Two-dimensional array copy of matrix elements.
         */
        public double[,] GetArrayCopy()
        {
            return (double[,])base.ToArray().Clone();
        }
        public double[] GetColumnPackedCopy()
        {
            return base.ToColumnWiseArray();
        }
        public void SetMatrix(int i0, int i1, int j0, int j1, Matrix X)
        {
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        this[i, j] = X[i - i0, j - j0];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices: " + e);
            }
        }
        public override void SetColumn(int columnIndex, double[] column)
        {
            base.SetColumn(columnIndex, column);
        }
        public override void SetColumn(int columnIndex, MathNet.Numerics.LinearAlgebra.Generic.Vector<double> column)
        {
            base.SetColumn(columnIndex, column);
        }
        public void SetColumn(int columnIndex, Vector column)
        {
            base.SetColumn(columnIndex, (MathNet.Numerics.LinearAlgebra.Generic.Vector<double>)column);
        }

        public Matrix TimesTranspose(Matrix B)
        {
            if (B.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentException("Matrix inner dimensions must agree.");
            }
            Matrix X = new Matrix(this.RowCount, B.RowCount);
            for (int j = 0; j < X.RowCount; j++)
            {
                double[] Browj = B.Row(j).ToArray();
                // multiply it with each row from A
                for (int i = 0; i < this.RowCount; i++)
                {
                    double[] Arowi = this.Row(i).ToArray();
                    double s = 0;
                    for (int k = 0; k < this.ColumnCount; k++)
                    {
                        s += Arowi[k] * Browj[k];
                    }
                    X[i, j] = s;
                }
            }
            return X;
        }


        /**
         * Linear algebraic matrix multiplication, A<sup>T</sup> * B
         * 
         * @param B another matrix
         * @return Matrix product, A<sup>T</sup> * B
         * @throws IllegalArgumentException Matrix inner dimensions must agree.
         */
        public Vector TransposeTimes(Vector B)
        {
            if (B.Count != RowCount)
            {
                throw new ArgumentException("Matrix inner dimensions must agree.");
            }
            Vector X = new Vector(this.ColumnCount);
            // multiply it with each row from A
            for (int i = 0; i < this.ColumnCount; i++)
            {
                double s = 0;
                for (int k = 0; k < RowCount; k++)
                {
                    s += this[k, i] * B[k];
                }
                X[i] = s;
            }
            return X;
        }
        public Matrix Copy()
        {
            return (Matrix)this.Clone();
        }
        public Matrix CompleteToOrthonormalBasis()
        {
            Matrix basis = Copy();
            Matrix result = null;
            for (int i = 0; i < this.RowCount; i++)
            {
                Matrix e_i = new Matrix(this.RowCount, 1);
                e_i[i, 0] = 1.0;
                bool li = basis.LinearlyIndependent(e_i);

                if (li)
                {
                    if (result == null)
                    {
                        result = e_i.Copy();
                    }
                    else
                    {
                        result = result.AppendColumns(e_i);
                    }
                    basis = basis.AppendColumns(e_i);
                }
            }
            basis = basis.Orthonormalize();
            return (Matrix)basis.SubMatrix(0, basis.RowCount - 1, ColumnCount, basis.ColumnCount - 1);
        }

        public Matrix Orthonormalize()
        {
            Matrix v = Copy();

            // FIXME: optimize - excess copying!
            for (int i = 1; i < ColumnCount; i++)
            {
                Vector u_i = Column(i);
                Vector sum = new Vector(RowCount);
                for (int j = 0; j < i; j++)
                {
                    Vector v_j = v.Column(j);
                    double scalar = u_i.TransposeTimes(v_j) / v_j.TransposeTimes(v_j);
                    sum.PlusTimesEquals(v_j, scalar);
                }
                Vector v_i = u_i - (sum);
                v.SetColumn(i, v_i);
            }

            v.NormalizeColumns();
            return v;
        }

        /**
         * Normalizes the columns of this matrix to length of 1.0.
         */
        public void NormalizeColumns()
        {
            for (int col = 0; col < ColumnCount; col++)
            {
                double norm = 0.0;
                for (int row = 0; row < RowCount; row++)
                {
                    norm = norm + (this[row, col] * this[row, col]);
                }
                norm = Math.Sqrt(norm);
                if (norm != 0)
                {
                    for (int row = 0; row < RowCount; row++)
                    {
                        this[row, col] /= norm;
                    }
                }
                // TODO: else: throw an exception?
            }
        }

        public bool LinearlyIndependent(Matrix columnMatrix)
        {
            if (columnMatrix.ColumnCount != 1)
            {
                throw new ArgumentException("a.getColumnDimension() != 1");
            }
            if (this.RowCount != columnMatrix.RowCount)
            {
                throw new ArgumentException("a.getRowDimension() != b.getRowDimension()");
            }
            if (this.ColumnCount + columnMatrix.ColumnCount > this.RowCount)
            {
                return false;
            }
            StringBuilder msg = new StringBuilder();

            double[,] a = new double[ColumnCount + 1, RowCount - 1];
            double[] b = new double[ColumnCount + 1];

            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (i < ColumnCount)
                    {
                        a[i, j] = this[j, i];
                    }
                    else
                    {
                        a[i, j] = columnMatrix[j, 0];
                    }
                }
            }

            for (int i = 0; i < b.Length; i++)
            {
                if (i < ColumnCount)
                {
                    b[i] = this[RowCount - 1, i];
                }
                else
                {
                    b[i] = columnMatrix[i, 0];
                }
            }

            LinearEquationSystem les = new LinearEquationSystem(a, b);
            les.SolveByTotalPivotSearch();

            double[,] coefficients = les.GetCoefficents();
            double[] rhs = les.GetRHS();

            if (msg != null)
            {
                msg.Append("\na' " + FormatUtil.Format(this.GetArrayRef()));
                msg.Append("\nb' " + FormatUtil.Format(columnMatrix.GetColumnPackedCopy()));

                msg.Append("\na " + FormatUtil.Format(a));
                msg.Append("\nb " + FormatUtil.Format(b));
                msg.Append("\nleq " + les.EquationsToString(4));
            }

            for (int i = 0; i < coefficients.Length; i++)
            {
                bool allCoefficientsZero = true;
                for (int j = 0; j < coefficients.GetLength(1); j++)
                {
                    double value = coefficients[i, j];
                    if (Math.Abs(value) > DELTA)
                    {
                        allCoefficientsZero = false;
                        break;
                    }
                }
                // allCoefficients=0 && rhs=0 -> linearly dependent
                if (allCoefficientsZero)
                {
                    double value = rhs[i];
                    if (Math.Abs(value) < DELTA)
                    {
                        if (msg != null)
                        {
                            msg.Append("\nvalue " + value + "[" + i + "]");
                            msg.Append("\nlinearly independent " + false);
                            Logging.GetLogger(this.GetType().Name).Debug(msg.ToString());
                        }
                        return false;
                    }
                }
            }

            if (msg != null)
            {
                msg.Append("\nlinearly independent " + true);
                Logging.GetLogger(this.GetType().Name).Debug(msg.ToString());
            }
            return true;
        }

        public Matrix AppendColumns(Matrix columns)
        {
            if (RowCount != columns.RowCount)
            {
                throw new ArgumentException("m.getRowDimension() != column.getRowDimension()");
            }

            Matrix result = new Matrix(RowCount, ColumnCount + columns.ColumnCount);
            for (int i = 0; i < result.ColumnCount; i++)
            {
                // FIXME: optimize - excess copying!
                if (i < ColumnCount)
                {
                    result.SetColumn(i, Column(i));
                }
                else
                {
                    result.SetColumn(i, columns.Column(i - ColumnCount));
                }
            }
            return result;
        }
    }
}
