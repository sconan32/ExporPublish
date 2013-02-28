using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Extenstions;
namespace Socona.Expor.Data
{

    public class Subspace<V>
    where V : IDataVector
    {
        /**
         * The dimensions building this subspace.
         */
        private BitArray dimensions = new BitArray(1000);

        /**
         * The dimensionality of this subspace.
         */
        private int count;

        /**
         * Creates a new one-dimensional subspace of the original data space.
         * 
         * @param dimension the dimension building this subspace
         */
        public Subspace(int count)
        {
            dimensions = new BitArray(count);
            this.count = count;
        }

        /**
         * Creates a new k-dimensional subspace of the original data space.
         * 
         * @param dimensions the dimensions building this subspace
         */
        public Subspace(BitArray dimensions)
        {
            this.dimensions = dimensions.Clone() as BitArray;
            count = dimensions.Count;
        }

        /**
         * Returns the BitArray representing the dimensions of this subspace.
         * 
         * @return the dimensions of this subspace
         */
        public BitArray GetDimensions()
        {
            return (BitArray)dimensions.Clone();
        }

        /**
         * Returns the dimensionality of this subspace.
         * 
         * @return the number of dimensions this subspace contains
         */
        public int Count
        {
            get { return count; }
        }

        /**
         * Joins this subspace with the specified subspace. The join is only
         * successful if both subspaces have the first k-1 dimensions in common (where
         * k is the number of dimensions) and the last dimension of this subspace is
         * less than the last dimension of the specified subspace.
         * 
         * @param other the subspace to join
         * @return the join of this subspace with the specified subspace if the join
         *         condition is fulfilled, null otherwise.
         * @see Subspace#joinLastDimensions(Subspace)
         */
        public Subspace<V> JoinWith(Subspace<V> other)
        {
            BitArray newDimensions = JoinLastDimensions(other);
            if (newDimensions == null)
            {
                return null;
            }

            return new Subspace<V>(newDimensions);
        }

        /**
         * Returns a string representation of this subspace by calling
         * {@link #ToString} with an empty prefix.
         * 
         * @return a string representation of this subspace
         */

        public override String ToString()
        {
            return ToString("");
        }

        /**
         * Returns a string representation of this subspace that contains the given
         * string prefix and the dimensions of this subspace.
         * 
         * @param pre a string prefix for each row of this string representation
         * @return a string representation of this subspace
         */
        public virtual String ToString(String pre)
        {
            StringBuilder result = new StringBuilder();
            result.Append(pre).Append("Dimensions: [");
            int start = dimensions.NextSetBitIndex(0);
            for (int d = start; d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                if (d != start)
                {
                    result.Append(", ");
                }
                result.Append(d + 1);
            }
            result.Append("]");
            return result.ToString();
        }

        /**
         * Returns a string representation of the dimensions of this subspace
         * separated by comma.
         * 
         * @return a string representation of the dimensions of this subspace
         */
        public String DimensonsToString()
        {
            return DimensonsToString(", ");
        }

        /**
         * Returns a string representation of the dimensions of this subspace.
         * 
         * @param sep the separator between the dimensions
         * @return a string representation of the dimensions of this subspace
         */
        public String DimensonsToString(String sep)
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            for (int dim = dimensions.NextSetBitIndex(0); dim >= 0; dim = dimensions.NextSetBitIndex(dim + 1))
            {
                if (result.Length == 1)
                {
                    result.Append(dim + 1);
                }
                else
                {
                    result.Append(sep).Append(dim + 1);
                }
            }
            result.Append("]");

            return result.ToString();
        }

        /**
         * Returns true if this subspace is a subspace of the specified subspace, i.e.
         * if the set of dimensions building this subspace are contained in the set of
         * dimensions building the specified subspace.
         * 
         * @param subspace the subspace to test
         * @return true if this subspace is a subspace of the specified subspace,
         *         false otherwise
         */
        public bool IsSubspace(Subspace<V> subspace)
        {
            if (this.count > subspace.count)
            {
                return false;
            }
            for (int d = dimensions.NextSetBitIndex(0); d >= 0; d = dimensions.NextSetBitIndex(d + 1))
            {
                if (!subspace.dimensions.Get(d))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Joins the dimensions of this subspace with the dimensions of the specified
         * subspace. The join is only successful if both subspaces have the first k-1
         * dimensions in common (where k is the number of dimensions) and the last
         * dimension of this subspace is less than the last dimension of the specified
         * subspace.
         * 
         * @param other the subspace to join
         * @return the joined dimensions of this subspace with the dimensions of the
         *         specified subspace if the join condition is fulfilled, null
         *         otherwise.
         */
        protected BitArray JoinLastDimensions(Subspace<V> other)
        {
            if (this.count != other.count)
            {
                return null;
            }
            //TODO: 这 里要修改构造函数的参数
            BitArray resultDimensions = new BitArray(1000);
            int last1 = -1, last2 = -1;

            for (int d1 = this.dimensions.NextSetBitIndex(0),
                d2 = other.dimensions.NextSetBitIndex(0); d1 >= 0 && d2 >= 0;
                d1 = this.dimensions.NextSetBitIndex(d1 + 1),
                d2 = other.dimensions.NextSetBitIndex(d2 + 1))
            {

                if (d1 == d2)
                {
                    resultDimensions.Set(d1, true);
                }
                last1 = d1;
                last2 = d2;
            }

            if (last1 < last2)
            {
                resultDimensions.Set(last1, true);
                resultDimensions.Set(last2, true);
                return resultDimensions;
            }
            else
            {
                return null;
            }
        }

        /**
         * Returns the hash code value of the {@link #dimensions} of this subspace.
         * 
         * @return a hash code value for this subspace
         */

        public override int GetHashCode()
        {
            return dimensions.GetHashCode();
        }

        /**
         * Indicates if the specified object is equal to this subspace, i.e. if the
         * specified object is a Subspace and is built of the same dimensions than
         * this subspace.
         * 
         * @see java.lang.Object#equals(java.lang.Object)
         */


        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            Subspace<V> other = (Subspace<V>)obj;
            return new DimensionComparator().Compare(this, other) == 0;
        }

        /**
         * A comparator for subspaces based on their involved dimensions. The
         * subspaces are ordered according to the ordering of their dimensions.
         * 
         * @author Elke Achtert
         */
        public class DimensionComparator : IComparer<Subspace<V>>
        {
            /**
             * Compares the two specified subspaces for order. If the two subspaces have
             * different dimensionalities a negative integer or a positive integer will
             * be returned if the dimensionality of the first subspace is less than or
             * greater than the dimensionality of the second subspace. Otherwise the
             * comparison works as follows: Let {@code d1} and {@code d2} be the first
             * occurrences of pairwise unequal dimensions in the specified subspaces.
             * Then a negative integer or a positive integer will be returned if {@code
             * d1} is less than or greater than {@code d2}. Otherwise the two subspaces
             * have equal dimensions and zero will be returned.
             * 
             */

            public int Compare(Subspace<V> s1, Subspace<V> s2)
            {
                if (s1 == s2 || s1.GetDimensions() == null && s2.GetDimensions() == null)
                {
                    return 0;
                }

                if (s1.GetDimensions() == null && s2.GetDimensions() != null)
                {
                    return -1;
                }

                if (s1.GetDimensions() != null && s2.GetDimensions() == null)
                {
                    return 1;
                }

                int compare = s1.Count - s2.Count;
                if (compare != 0)
                {
                    return compare;
                }

                for (int d1 = s1.GetDimensions().NextSetBitIndex(0), 
                    d2 = s2.GetDimensions().NextSetBitIndex(0);
                    d1 >= 0 && d2 >= 0;
                    d1 = s1.GetDimensions().NextSetBitIndex(d1 + 1),
                    d2 = s2.GetDimensions().NextSetBitIndex(d2 + 1))
                {
                    if (d1 != d2)
                    {
                        return d1 - d2;
                    }
                }
                return 0;
            }
        }
    }

}
