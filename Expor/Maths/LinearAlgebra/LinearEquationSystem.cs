using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;
using Socona.Expor.Utilities.Pairs;
using Socona.Log;

namespace Socona.Expor.Maths.LinearAlgebra
{

    public class LinearEquationSystem
    {
        /**
         * Logger.
         */
        private static Logging logger = Logging.GetLogger(typeof(LinearEquationSystem));

        /**
         * Indicates trivial pivot search strategy.
         */
        private const int TRIVAL_PIVOT_SEARCH = 0;

        /**
         * Indicates total pivot search strategy.
         */
        private const int TOTAL_PIVOT_SEARCH = 1;

        /**
         * Indicates if linear equation system is solvable.
         */
        private bool solvable;

        /**
         * Indicates if solvability has been checked.
         */
        private bool solved;

        /**
         * The rank of the coefficient matrix.
         */
        private int rank;

        /**
         * The matrix of coefficients.
         */
        private double[,] coeff;

        /**
         * The right hand side of the equation system.
         */
        private double[] rhs;

        /**
         * Encodes row permutations, row i is at position row[i].
         */
        private int[] row;

        /**
         * Encodes column permutations, column j is at position col[j].
         */
        private int[] col;

        /**
         * Holds the special solution vector.
         */
        private double[] x_0;

        /**
         * Holds the space of solutions of the homogeneous linear equation system.
         */
        private double[,] u;

        /**
         * Indicates if linear equation system is in reduced row echelon form.
         */
        private bool reducedRowEchelonForm;

        /**
         * Constructs a linear equation system with given coefficient matrix
         * <code>a</code> and right hand side <code>b</code>.
         * 
         * @param a the matrix of the coefficients of the linear equation system
         * @param b the right hand side of the linear equation system
         */
        public LinearEquationSystem(double[,] a, double[] b)
        {
            if (a == null)
            {
                throw new ArgumentException("Coefficient array is null!");
            }
            if (b == null)
            {
                throw new ArgumentException("Right hand side is null!");
            }
            if (a.Length != b.Length)
            {
                throw new ArgumentException("Coefficient matrix and right hand side " + "differ in row dimensionality!");
            }

            coeff = a;
            rhs = b;
            row = new int[coeff.Length];
            for (int i = 0; i < coeff.Length; i++)
            {
                row[i] = i;
            }
            col = new int[coeff.GetLength(1)];
            for (int j = 0; j < coeff.GetLength(1); j++)
            {
                col[j] = j;
            }
            rank = 0;
            x_0 = null;
            solved = false;
            solvable = false;
            reducedRowEchelonForm = false;
        }

        /**
         * Constructs a linear equation system with given coefficient matrix
         * <code>a</code> and right hand side <code>b</code>.
         * 
         * @param a the matrix of the coefficients of the linear equation system
         * @param b the right hand side of the linear equation system
         * @param rowPermutations the row permutations, row i is at position row[i]
         * @param columnPermutations the column permutations, column i is at position
         *        column[i]
         */
        public LinearEquationSystem(double[,] a, double[] b, int[] rowPermutations, int[] columnPermutations)
        {
            if (a == null)
            {
                throw new ArgumentException("Coefficient array is null!");
            }
            if (b == null)
            {
                throw new ArgumentException("Right hand side is null!");
            }
            if (a.Length != b.Length)
            {
                throw new ArgumentException("Coefficient matrix and right hand side " + "differ in row dimensionality!");
            }
            if (rowPermutations.Length != a.Length)
            {
                throw new ArgumentException("Coefficient matrix and row permutation array " + "differ in row dimensionality!");
            }
            if (columnPermutations.Length != a.GetLength(1))
            {
                throw new ArgumentException("Coefficient matrix and column permutation array " + "differ in column dimensionality!");
            }

            coeff = a;
            rhs = b;
            this.row = rowPermutations;
            this.col = columnPermutations;
            rank = 0;
            x_0 = null;
            solved = false;
            solvable = false;
            reducedRowEchelonForm = false;
        }

        /**
         * Returns a copy of the coefficient array of this linear equation system.
         * 
         * @return a copy of the coefficient array of this linear equation system
         */
        public double[,] GetCoefficents()
        {
            return (double[,])coeff.Clone();
        }

        /**
         * Returns a copy of the right hand side of this linear equation system.
         * 
         * @return a copy of the right hand side of this linear equation system
         */
        public double[] GetRHS()
        {
            return (double[])rhs.Clone();
        }

        /**
         * Returns a copy of the row permutations, row i is at position row[i].
         * 
         * @return a copy of the row permutations
         */
        public int[] GetRowPermutations()
        {
            return (int[])row.Clone();
        }

        /**
         * Returns a copy of the column permutations, column i is at position
         * column[i].
         * 
         * @return a copy of the column permutations
         */
        public int[] GetColumnPermutations()
        {
            return (int[])col.Clone();
        }

        /**
         * Tests if system has already been tested for solvability.
         * 
         * @return true if a solution has already been computed, false otherwise.
         */
        public bool isSolved()
        {
            return solved;
        }

        /**
         * Solves this linear equation system by total pivot search.
         * "Total pivot search" takes as pivot element the element in the current
         * column having the biggest value. If we have: <br>
         * <code>
         * ( a_11 &nbsp;&nbsp;&nbsp;&nbsp; ... &nbsp;&nbsp;&nbsp;&nbsp; a_1n      ) <br>
         * (  0 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  ...   &nbsp;&nbsp;&nbsp;&nbsp; a_2n      ) <br>
         * (  0 ... a_ii     &nbsp;&nbsp;&nbsp; ... a_in      )<br>
         * (  0 ... a_(i+1)i ... a_(i+1)n  ) <br>
         * (  0 ... a_ni     &nbsp;&nbsp;&nbsp; ... a_nn      ) <br>
           * </code> Then we search for x,y in {i,...n}, so that |a_xy| > |a_ij|
         */
        public void SolveByTotalPivotSearch()
        {
            solve(TOTAL_PIVOT_SEARCH);
        }

        /**
         * Solves this linear equation system by trivial pivot search.
         * "Trivial pivot search" takes as pivot element the next element in the
         * current column beeing non zero.
         */
        public void SolveByTrivialPivotSearch()
        {
            solve(TRIVAL_PIVOT_SEARCH);
        }

        /**
         * Checks if a solved system is solvable.
         * 
         * @return true if this linear equation system is solved and solvable
         */
        public bool isSolvable()
        {
            return solvable && solved;
        }

        /**
         * Returns a string representation of this equation system.
         * 
         * @param prefix the prefix of each line
         * @param fractionDigits the number of fraction digits for output accuracy
         * @return a string representation of this equation system
         */
        public String EquationsToString(String prefix, int fractionDigits)
        {
            //DecimalFormat nf = new DecimalFormat();
            //nf.setMinimumFractionDigits(fractionDigits);
            //nf.setMaximumFractionDigits(fractionDigits);
            //nf.setDecimalFormatSymbols(new DecimalFormatSymbols(Locale.US));
            //nf.setNegativePrefix("");
            //nf.setPositivePrefix("");
            NumberFormatInfo nf = new NumberFormatInfo();
            nf.NumberDecimalDigits = fractionDigits;
            nf.NegativeSign = string.Empty;
            nf.PositiveSign = string.Empty;
            return EquationsToString(prefix, nf);
        }

        /**
         * Returns a string representation of this equation system.
         * 
         * @param prefix the prefix of each line
         * @param nf the number Format
         * @return a string representation of this equation system
         */
        public String EquationsToString(String prefix, NumberFormatInfo nf)
        {
            if ((coeff == null) || (rhs == null) || (row == null) || (col == null))
            {
                throw new NullReferenceException();
            }

            int[] coeffDigits = maxIntegerDigits(coeff);
            int rhsDigits = maxIntegerDigits(rhs);

            StringBuilder buffer = new StringBuilder();
            buffer.Append(prefix).Append("\n").Append(prefix);
            for (int i = 0; i < coeff.Length; i++)
            {
                for (int j = 0; j < coeff.GetLength(1); j++)
                {
                    Format(nf, buffer, coeff[row[i], col[j]], coeffDigits[col[j]]);
                    buffer.Append(" * x_" + col[j]);
                }
                buffer.Append(" =");
                Format(nf, buffer, rhs[row[i]], rhsDigits);

                if (i < coeff.Length - 1)
                {
                    buffer.Append("\n").Append(prefix);
                }
                else
                {
                    buffer.Append("\n").Append(prefix);
                }
            }
            return buffer.ToString();
        }

        /**
         * Returns a string representation of this equation system.
         * 
         * @param nf the number Format
         * @return a string representation of this equation system
         */
        public String EquationsToString(NumberFormatInfo nf)
        {
            return EquationsToString("", nf);
        }

        /**
         * Returns a string representation of this equation system.
         * 
         * @param fractionDigits the number of fraction digits for output accuracy
         * @return a string representation of this equation system
         */
        public String EquationsToString(int fractionDigits)
        {
            return EquationsToString("", fractionDigits);
        }

        /**
         * Returns a string representation of the solution of this equation system.
         * 
         * @param fractionDigits precision
         * 
         * @return a string representation of the solution of this equation system
         */
        public String SolutionToString(int fractionDigits)
        {
            if (!isSolvable())
            {
                throw new InvalidOperationException("System is not solvable!");
            }

            //DecimalFormat nf = new DecimalFormat();
            //nf.setMinimumFractionDigits(fractionDigits);
            //nf.setMaximumFractionDigits(fractionDigits);
            //nf.setDecimalFormatSymbols(new DecimalFormatSymbols(Locale.US));
            //nf.setNegativePrefix("");
            //nf.setPositivePrefix("");
            NumberFormatInfo nf = new NumberFormatInfo();
            nf.NumberDecimalDigits = fractionDigits;
            nf.NegativeSign = string.Empty;
            nf.PositiveSign = string.Empty;

            int row = coeff.GetLength(1) / 2;
            int param = u.Length;
            int paramsDigits = integerDigits(param);

            int x0Digits = maxIntegerDigits(x_0);
            int[] uDigits = maxIntegerDigits(u);
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < x_0.Length; i++)
            {
                double value = x_0[i];
                Format(nf, buffer, value, x0Digits);
                for (int j = 0; j < u.GetLength(1); j++)
                {
                    if (i == row)
                    {
                        buffer.Append("  +  a_" + j + " * ");
                    }
                    else
                    {
                        buffer.Append("          ");
                        for (int d = 0; d < paramsDigits; d++)
                        {
                            buffer.Append(" ");
                        }
                    }
                    Format(nf, buffer, u[i, j], uDigits[j]);
                }
                buffer.Append("\n");
            }
            return buffer.ToString();
        }

        /**
         * Brings this linear equation system into reduced row echelon form with
         * choice of pivot method.
         * 
         * @param method the pivot search method to use
         */
        private void ReducedRowEchelonForm(int method)
        {
            int rows = coeff.Length;
            int cols = coeff.GetLength(1);

            int k = -1; // denotes current position on diagonal
            int pivotRow; // row index of pivot element
            int pivotCol; // column index of pivot element
            double pivot; // value of pivot element

            // main loop, transformation to reduced row echelon form
            bool exitLoop = false;

            while (!exitLoop)
            {
                k++;

                // pivot search for entry in remaining matrix
                // (depends on chosen method in switch)
                // store position in pivotRow, pivotCol

                // TODO: Note that we're using "row, col", whereas "col, row" would be
                // more common?
                IntIntPair pivotPos = new IntIntPair(0, 0);
                IntIntPair currPos = new IntIntPair(k, k);

                switch (method)
                {
                    case TRIVAL_PIVOT_SEARCH:
                        pivotPos = nonZeroPivotSearch(k);
                        break;
                    case TOTAL_PIVOT_SEARCH:
                        pivotPos = totalPivotSearch(k);
                        break;
                }
                pivotRow = pivotPos.first;
                pivotCol = pivotPos.second;
                pivot = coeff[this.row[pivotRow], col[pivotCol]];

                if (logger.IsDebugging)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append("equations ").Append(EquationsToString(4));
                    msg.Append("  *** pivot at (").Append(pivotRow).Append(",").Append(pivotCol).Append(") = ").Append(pivot).Append("\n");
                    logger.Debug(msg.ToString());
                }

                // permute rows and columns to Get this entry onto
                // the diagonal
                permutePivot(pivotPos, currPos);

                // test conditions for exiting loop
                // after this iteration
                // reasons are: Math.Abs(pivot) == 0
                if ((Math.Abs(pivot) <= Matrix.DELTA))
                {
                    exitLoop = true;
                }

                // pivoting only if Math.Abs(pivot) > 0
                // and k <= m - 1
                if ((Math.Abs(pivot) > Matrix.DELTA))
                {
                    rank++;
                    pivotOperation(k);
                }

                // test conditions for exiting loop
                // after this iteration
                // reasons are: k == rows-1 : no more rows
                // k == cols-1 : no more columns
                if (k == rows - 1 || k == cols - 1)
                {
                    exitLoop = true;
                }
            }// end while

            reducedRowEchelonForm = true;
        }

        /**
         * Method for total pivot search, searches for x,y in {k,...n}, so that |a_xy|
         * > |a_ij|
         * 
         * @param k search starts at entry (k,k)
         * @return the position of the found pivot element
         */
        private IntIntPair totalPivotSearch(int k)
        {
            double max = 0;
            int i, j, pivotRow = k, pivotCol = k;
            double absValue;
            for (i = k; i < coeff.Length; i++)
            {
                for (j = k; j < coeff.GetLength(1); j++)
                {
                    // compute absolute value of
                    // current entry in absValue
                    absValue = Math.Abs(coeff[row[i], col[j]]);

                    // compare absValue with value max
                    // found so far
                    if (max < absValue)
                    {
                        // remember new value and position
                        max = absValue;
                        pivotRow = i;
                        pivotCol = j;
                    }// end if
                }// end for j
            }// end for k
            return new IntIntPair(pivotRow, pivotCol);
        }

        /**
         * Method for trivial pivot search, searches for non-zero entry.
         * 
         * @param k search starts at entry (k,k)
         * @return the position of the found pivot element
         */
        private IntIntPair nonZeroPivotSearch(int k)
        {

            int i, j;
            double absValue;
            for (i = k; i < coeff.Length; i++)
            {
                for (j = k; j < coeff.GetLength(1); j++)
                {
                    // compute absolute value of
                    // current entry in absValue
                    absValue = Math.Abs(coeff[row[i], col[j]]);

                    // check if absValue is non-zero
                    if (absValue > 0)
                    { // found a pivot element
                        return new IntIntPair(i, j);
                    }// end if
                }// end for j
            }// end for k
            return new IntIntPair(k, k);
        }

        /**
         * permutes two matrix rows and two matrix columns
         * 
         * @param pos1 the fist position for the permutation
         * @param pos2 the second position for the permutation
         */
        private void permutePivot(IntIntPair pos1, IntIntPair pos2)
        {
            int r1 = pos1.first;
            int c1 = pos1.second;
            int r2 = pos2.first;
            int c2 = pos2.second;
            int index;
            index = row[r2];
            row[r2] = row[r1];
            row[r1] = index;
            index = col[c2];
            col[c2] = col[c1];
            col[c1] = index;
        }

        /**
         * performs a pivot operation
         * 
         * @param k pivoting takes place below (k,k)
         */
        private void pivotOperation(int k)
        {
            double pivot = coeff[row[k], col[k]];

            // pivot row: set pivot to 1
            coeff[row[k], col[k]] = 1;
            for (int i = k + 1; i < coeff.GetLength(1); i++)
            {
                coeff[row[k], col[i]] /= pivot;
            }
            rhs[row[k]] /= pivot;

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("set pivot element to 1 ").Append(EquationsToString(4));
                logger.Debug(msg.ToString());
            }

            // for (int i = k + 1; i < coeff.Length; i++) {
            for (int i = 0; i < coeff.Length; i++)
            {
                if (i == k)
                {
                    continue;
                }

                // compute factor
                double q = coeff[row[i], col[k]];

                // modify entry a[i,k], i <> k
                coeff[row[i], col[k]] = 0;

                // modify entries a[i,j], i > k fixed, j = k+1...n-1
                for (int j = k + 1; j < coeff.GetLength(1); j++)
                {
                    coeff[row[i], col[j]] = coeff[row[i], col[j]] - coeff[row[k], col[j]] * q;
                }// end for j

                // modify right-hand-side
                rhs[row[i]] = rhs[row[i]] - rhs[row[k]] * q;
            }// end for k

            if (logger.IsDebugging)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("after pivot operation ").Append(EquationsToString(4));
                logger.Debug(msg.ToString());
            }
        }

        /**
         * solves linear system with the chosen method
         * 
         * @param method the pivot search method
         */
        private void solve(int method)
        {
            // solution exists
            if (solved)
            {
                return;
            }

            // bring in reduced row echelon form
            if (!reducedRowEchelonForm)
            {
                ReducedRowEchelonForm(method);
            }

            if (!isSolvable(method))
            {
                if (logger.IsDebugging)
                {
                    logger.Debug("Equation system is not solvable!");
                }
                return;
            }

            // compute one special solution
            int cols = coeff.GetLength(1);
            List<Int32> boundIndices = new List<Int32>();
            x_0 = new double[cols];
            for (int i = 0; i < coeff.Length; i++)
            {
                for (int j = i; j < coeff.GetLength(1); j++)
                {
                    if (coeff[row[i], col[j]] == 1)
                    {
                        x_0[col[i]] = rhs[row[i]];
                        boundIndices.Add(col[i]);
                        break;
                    }
                }
            }
            List<Int32> freeIndices = new List<Int32>();
            for (int i = 0; i < coeff.GetLength(1); i++)
            {
                if (boundIndices.Contains(i))
                {
                    continue;
                }
                freeIndices.Add(i);
            }

            StringBuilder msg = new StringBuilder();
            if (logger.IsDebugging)
            {
                msg.Append("\nSpecial solution x_0 = [").Append(FormatUtil.Format(x_0, ",", 4)).Append("]");
                msg.Append("\nbound Indices ").Append(boundIndices);
                msg.Append("\nfree Indices ").Append(freeIndices);
            }

            // compute solution space of homogeneous linear equation system
            Int32[] freeParameters = freeIndices.ToArray();
            Int32[] boundParameters = boundIndices.ToArray();
            Array.Sort(boundParameters);
            int freeIndex = 0;
            int boundIndex = 0;
            u = new double[cols, freeIndices.Count];

            for (int j = 0; j < u.GetLength(1); j++)
            {
                for (int i = 0; i < u.Length; i++)
                {
                    if (freeIndex < freeParameters.Length && i == freeParameters[freeIndex])
                    {
                        u[i, j] = 1;
                    }
                    else if (boundIndex < boundParameters.Length && i == boundParameters[boundIndex])
                    {
                        u[i, j] = -coeff[row[boundIndex], freeParameters[freeIndex]];
                        boundIndex++;
                    }
                }
                freeIndex++;
                boundIndex = 0;

            }

            if (logger.IsDebugging)
            {
                msg.Append("\nU");
                //foreach (double[] anU in u)
                for (int ui = 0; ui < u.Length; ui++)
                {
                    msg.Append("\n");
                    for (int uj = 0; uj < u.GetLength(1); uj++)
                    {

                        msg.Append(FormatUtil.Format(u[ui, uj], 4)).Append(",");
                    }
                }
                logger.Debug(msg.ToString());
            }

            solved = true;
        }

        /**
         * Checks solvability of this linear equation system with the chosen method.
         * 
         * @param method the pivot search method
         * @return true if linear system in solvable
         */
        private bool isSolvable(int method)
        {
            if (solved)
            {
                return solvable;
            }

            if (!reducedRowEchelonForm)
            {
                ReducedRowEchelonForm(method);
            }

            // test if rank(coeff) == rank(coeff|rhs)
            for (int i = rank; i < rhs.Length; i++)
            {
                if (Math.Abs(rhs[row[i]]) > Matrix.DELTA)
                {
                    solvable = false;
                    return false; // not solvable
                }
            }

            solvable = true;
            return true;
        }

        /**
         * Returns the maximum integer digits in each column of the specified values.
         * 
         * @param values the values array
         * @return the maximum integer digits in each column of the specified values
         */
        private int[] maxIntegerDigits(double[,] values)
        {
            int[] digits = new int[values.GetLength(1)];
            for (int j = 0; j < values.GetLength(1); j++)
            {
                //foreach (double[] value in values)
                for(int i=0;i<values.Length;i++)
                {
                    digits[j] = Math.Max(digits[j], integerDigits(values[i,j]));
                }
            }
            return digits;
        }

        /**
         * Returns the maximum integer digits of the specified values.
         * 
         * @param values the values array
         * @return the maximum integer digits of the specified values
         */
        private int maxIntegerDigits(double[] values)
        {
            int digits = 0;
            foreach (double value in values)
            {
                digits = Math.Max(digits, integerDigits(value));
            }
            return digits;
        }

        /**
         * Returns the integer digits of the specified double value.
         * 
         * @param d the double value
         * @return the integer digits of the specified double value
         */
        private int integerDigits(double d)
        {
            double value = Math.Abs(d);
            if (value < 10)
            {
                return 1;
            }
            return (int)Math.Log10(value) + 1;
        }

        /**
         * Helper method for output of equations and solution. Appends the specified
         * double value to the given string buffer according the number Format and the
         * maximum number of integer digits.
         * 
         * @param nf the number Format
         * @param buffer the string buffer to Append the value to
         * @param value the value to Append
         * @param maxIntegerDigits the maximum number of integer digits
         */
        private void Format(NumberFormatInfo nf, StringBuilder buffer, double value, int maxIntegerDigits)
        {
            if (value >= 0)
            {
                buffer.Append(" + ");
            }
            else
            {
                buffer.Append(" - ");
            }
            int digits = maxIntegerDigits - integerDigits(value);
            for (int d = 0; d < digits; d++)
            {
                buffer.Append(" ");
            }
            buffer.Append(Math.Abs(value).ToString("N", nf));
        }

        /**
         * Return dimensionality of spanned subspace.
         * 
         * @return dim
         */
        public int Subspacedim()
        {
            return coeff.GetLength(1) - coeff.Length;
        }

    }
}
