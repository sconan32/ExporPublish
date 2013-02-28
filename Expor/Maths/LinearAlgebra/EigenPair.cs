using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities;

namespace Socona.Expor.Maths.LinearAlgebra
{

    public class EigenPair : IComparable<EigenPair>
    {
        /**
         * The eigenvector as a matrix.
         */
        private Vector eigenvector;

        /**
         * The corresponding eigenvalue.
         */
        private double eigenvalue;

        /**
         * Creates a new EigenPair object.
         * 
         * @param eigenvector the eigenvector as a matrix
         * @param eigenvalue the corresponding eigenvalue
         */
        public EigenPair(Vector eigenvector, double eigenvalue)
        {
            this.eigenvalue = eigenvalue;
            this.eigenvector = eigenvector;
        }

        /**
         * Compares this object with the specified object for order. Returns a
         * negative integer, zero, or a positive integer as this object's eigenvalue
         * is greater than, equal to, or less than the specified object's eigenvalue.
         * 
         * @param o the Eigenvector to be compared.
         * @return a negative integer, zero, or a positive integer as this object's
         *         eigenvalue is greater than, equal to, or less than the specified
         *         object's eigenvalue.
         */

        public int CompareTo(EigenPair o)
        {
            if (this.eigenvalue < o.eigenvalue)
            {
                return -1;
            }
            if (this.eigenvalue > o.eigenvalue)
            {
                return +1;
            }
            return 0;
        }

        /**
         * Returns the eigenvector.
         * 
         * @return the eigenvector
         */
        public Vector Eigenvector
        {
            get { return eigenvector; }
        }

        /**
         * Returns the eigenvalue.
         * 
         * @return the eigenvalue
         */
        public double Eigenvalue
        {
            get { return eigenvalue; }
        }

        /**
         * Returns a string representation of this EigenPair.
         * 
         * @return a string representation of this EigenPair
         */

        public override String ToString()
        {
            return "(ew = " + FormatUtil.Format(eigenvalue) + ", ev = [" + FormatUtil.Format(eigenvector) + "])";
        }
    }
}
