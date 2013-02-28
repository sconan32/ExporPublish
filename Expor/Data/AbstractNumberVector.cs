using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.DataStructures.ArrayLike;

namespace Socona.Expor.Data
{

    /**
     * AbstractNumberVector is an abstract implementation of FeatureVector.
     * 
     * @author Arthur Zimek
     * @param <T> the type of number, this AbstractNumberVector consists of (i.e., a
     *        AbstractNumberVector {@code v} of type {@code V} and dimensionality
     *        {@code d} is an element of {@code N}<sup>{@code d}</sup>)
     */
    public abstract class AbstractNumberVector<T> : INumberVector
        where T : struct,IEquatable<T>, IComparable<T>, IConvertible
    {

        /**
         * The String to separate attribute values in a String that represents the
         * values.
         */
        public readonly static String ATTRIBUTE_SEPARATOR = " ";

        /**
         * An Object obj is equal to this AbstractNumberVector if it is an instance of
         * the same runtime class and is of the identical dimensionality and the
         * values of this AbstractNumberVector are equal to the values of obj in all
         * dimensions, respectively.
         * 
         * @param obj another Object
         * @return true if the specified Object is an instance of the same runtime
         *         class and is of the identical dimensionality and the values of this
         *         AbstractNumberVector are equal to the values of obj in all
         *         dimensions, respectively
         */

        public override bool Equals(Object obj)
        {
            if (this.GetType().IsInstanceOfType(obj))
            {
                AbstractNumberVector<T> rv = obj as AbstractNumberVector<T>;
                bool equal = (this.Count == rv.Count);
                for (int i = 0; i < Count && equal; i++)
                {
                    equal &= this[i].Equals(rv[i]);
                }
                return equal;
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


        public T GetMin(int dimension)
        {
            return this[dimension];
        }


        public T GetMax(int dimension)
        {
            return this[dimension];
        }


        public abstract Expor.Maths.LinearAlgebra.Vector GetColumnVector();

        public abstract int Count { get; }

        public abstract T this[int ind]
        { get; set; }

        public object Get(int dim)
        {
            return this[dim];
        }


        public abstract IDataVector NewFeatureVector(IList<object> array, IArrayAdapter adapter);

        public abstract INumberVector NewNumberVector(double[] array);


        double INumberVector.this[int dimension]
        {
            get
            {
                return Convert.ToDouble(this[dimension]); ;
            }
            set
            {
                T tmp = (dynamic)value;
                this[dimension] = tmp;
            }
        }



        // public abstract INumberVector NewNumberVector(T[] values);


        double Socona.Expor.Data.Spatial.ISpatialComparable.GetMax(int dim)
        {
            return Convert.ToDouble(this.GetMax(dim));
        }
        double Socona.Expor.Data.Spatial.ISpatialComparable.GetMin(int dim)
        {
            return Convert.ToDouble(this.GetMin(dim));
        }

    }
}
