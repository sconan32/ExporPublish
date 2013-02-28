using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.LinearAlgebra
{

    public class SortedEigenPairs
    {
        /**
         * The array of eigenpairs.
         */
        private EigenPair[] eigenPairs;

        /**
         * Creates a new empty SortedEigenPairs object. Can only be called from the
         * copy() method.
         */
        private SortedEigenPairs()
        {
            // nothing to do here.
        }

        /**
         * Creates a new SortedEigenPairs object from the specified eigenvalue
         * decomposition. The eigenvectors are sorted according to the specified
         * order.
         * 
         * @param evd the underlying eigenvalue decomposition
         * @param ascending a bool that indicates ascending order
         */
        public SortedEigenPairs(EigenvalueDecomposition evd, bool ascending)
        {
            double[] eigenvalues = evd.getRealEigenvalues();
            Matrix eigenvectors = evd.getV();

            this.eigenPairs = new EigenPair[eigenvalues.Length];
            for (int i = 0; i < eigenvalues.Length; i++)
            {
                double e = Math.Abs(eigenvalues[i]);
                Vector v = eigenvectors.Column(i);
                eigenPairs[i] = new EigenPair(v, e);
            }

            EigenPairComparer comp = new EigenPairComparer();

            Array.Sort(eigenPairs, comp);
        }

        internal class EigenPairComparer : IComparer<EigenPair>
        {
            bool ascending;
            public EigenPairComparer(bool ascending = false)
            {
                this.ascending = ascending;
            }
            public int Compare(EigenPair o1, EigenPair o2)
            {
                int comp = o1.CompareTo(o2);
                if (!ascending)
                {
                    comp = -1 * comp;
                }
                return comp;
            }
        }
        /**
         * Creates a new SortedEigenPairs object from the specified list. The
         * eigenvectors are sorted in descending order.
         * 
         * @param eigenPairs the eigenpairs to be sorted
         */
        public SortedEigenPairs(List<EigenPair> eigenPairs)
        {
            EigenPairComparer comp = new EigenPairComparer(true);

            this.eigenPairs = eigenPairs.ToArray();
            Array.Sort(this.eigenPairs, comp);
        }

        /**
         * Returns the sorted eigenvalues.
         * 
         * @return the sorted eigenvalues
         */
        public double[] EigenValues()
        {
            double[] eigenValues = new double[eigenPairs.Length];
            for (int i = 0; i < eigenPairs.Length; i++)
            {
                EigenPair eigenPair = eigenPairs[i];
                eigenValues[i] = eigenPair.Eigenvalue;
            }
            return eigenValues;
        }

        /**
         * Returns the sorted eigenvectors.
         * 
         * @return the sorted eigenvectors
         */
        public Matrix EigenVectors()
        {
            Matrix eigenVectors = new Matrix(eigenPairs.Length, eigenPairs.Length);
            for (int i = 0; i < eigenPairs.Length; i++)
            {
                EigenPair eigenPair = eigenPairs[i];
                eigenVectors.SetColumn(i, eigenPair.Eigenvector.ToArray());
            }
            return eigenVectors;
        }

        /**
         * Returns the first <code>n</code> sorted eigenvectors as a matrix.
         * 
         * @param n the number of eigenvectors (columns) to be returned
         * @return the first <code>n</code> sorted eigenvectors
         */
        public Matrix EigenVectors(int n)
        {
            Matrix eigenVectors = new Matrix(eigenPairs.Length, n);
            for (int i = 0; i < n; i++)
            {
                EigenPair eigenPair = eigenPairs[i];
                eigenVectors.SetColumn(i, eigenPair.Eigenvector.ToArray());
            }
            return eigenVectors;
        }

        /**
         * Returns the last <code>n</code> sorted eigenvectors as a matrix.
         * 
         * @param n the number of eigenvectors (columns) to be returned
         * @return the last <code>n</code> sorted eigenvectors
         */
        public Matrix ReverseEigenVectors(int n)
        {
            Matrix eigenVectors = new Matrix(eigenPairs.Length, n);
            for (int i = 0; i < n; i++)
            {
                EigenPair eigenPair = eigenPairs[eigenPairs.Length - 1 - i];
                eigenVectors.SetColumn(i, eigenPair.Eigenvector.ToArray());
            }
            return eigenVectors;
        }

        /**
         * Returns the eigenpair at the specified index.
         * 
         * @param index the index of the eigenpair to be returned
         * @return the eigenpair at the specified index
         */
        public EigenPair GetEigenPair(int index)
        {
            return eigenPairs[index];
        }

        /**
         * Returns the number of the eigenpairs.
         * 
         * @return the number of the eigenpairs
         */
        public int Count
        {
            get { return eigenPairs.Length; }
        }

        /**
         * Returns a string representation of this EigenPair.
         * 
         * @return a string representation of this EigenPair
         */

        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (EigenPair eigenPair in eigenPairs)
            {
                result.Append("\n").Append(eigenPair);
            }
            return result.ToString();
        }

        /**
         * Returns a deep copy of this object
         * 
         * @return new copy
         */
        public SortedEigenPairs Copy()
        {
            SortedEigenPairs cp = new SortedEigenPairs();
            cp.eigenPairs = (EigenPair[])this.eigenPairs.Clone();
            return cp;
        }
    }
}
