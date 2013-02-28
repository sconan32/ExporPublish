using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.LinearAlgebra.Pca
{
    public class PCAResult
    {
        /**
         * The eigenpairs in decreasing order.
         */
        private SortedEigenPairs eigenPairs;

        /**
         * The eigenvalues in decreasing order.
         */
        private double[] eigenvalues;

        /**
         * The eigenvectors in decreasing order to their corresponding eigenvalues.
         */
        private Matrix eigenvectors;

        /**
         * Build a PCA result object.
         * 
         * @param eigenvalues Eigenvalues
         * @param eigenvectors Eigenvector matrix
         * @param eigenPairs Eigenpairs
         */

        public PCAResult(double[] eigenvalues, Matrix eigenvectors, SortedEigenPairs eigenPairs)
        {

            this.eigenPairs = eigenPairs;
            this.eigenvalues = eigenvalues;
            this.eigenvectors = eigenvectors;
        }

        /**
         * Build a PCA result from an existing set of EigenPairs.
         * 
         * @param eigenPairs existing eigenpairs
         */
        public PCAResult(SortedEigenPairs eigenPairs)
            : base()
        {

            // TODO: we might want to postpone the instantiation of eigenvalue and
            // eigenvectors.
            this.eigenPairs = eigenPairs;
            this.eigenvalues = eigenPairs.EigenValues();
            this.eigenvectors = eigenPairs.EigenVectors();
        }

        /**
         * Returns the matrix of eigenvectors of the object to which this PCA belongs
         * to.
         * 
         * @return the matrix of eigenvectors
         */
        public Matrix Eigenvectors
        {
            get { return eigenvectors; }
        }

        /**
         * Returns the eigenvalues of the object to which this PCA belongs to in
         * decreasing order.
         * 
         * @return the eigenvalues
         */
        public double[] Eigenvalues
        {
            get { return eigenvalues; }
        }

        /**
         * Returns the eigenpairs of the object to which this PCA belongs to
         * in decreasing order.
         * 
         * @return the eigenpairs
         */
        public SortedEigenPairs EigenPairs
        {
            get { return eigenPairs; }
        }

        /**
         * Returns the number of eigenvectors stored
         * 
         * @return length
         */
        public int Count
        {
            get { return eigenPairs.Count; }
        }
    }
}
