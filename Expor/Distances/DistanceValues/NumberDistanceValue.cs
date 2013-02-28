using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Distances.DistanceValues
{

    /**
     * Provides a Distance for a number-valued distance.
     * 
     * @author Elke Achtert
     * 
     * @apiviz.landmark
     * @apiviz.composedOf Number
     * 
     * @param <D> the () type of NumberDistance used
     * @param <N> the type of Number used (e.g. Double, Integer, Float, etc.)
     */
    public abstract class NumberDistanceValue<N> : AbstractDistanceValue
    {
        /**
         * Constructs a new NumberDistance object that represents the value argument.
         */
        public NumberDistanceValue()
            : base()
        {

        }


        /**
         * Returns the hash code for this NumberDistance, which is the hash code of
         * its value.
         * 
         * @return the hash code of the value
         */

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /**
         * Compares this NumberDistance with the given NumberDistance wrt the
         * represented value.
         * <p/>
         * <code>d1.compareTo(d2)</code> is the same as
         * {@link Double#compare(double,double) Double.compare(d1.value.doubleValue(),
         * d2.value.doubleValue())}. Subclasses may need to overwrite this method if
         * necessary.
         * 
         * @param other Other object
         * @return a negative integer, zero, or a positive integer as the value of
         *         this NumberDistance is less than, equal to, or greater than the
         *         value of the specified NumberDistance.
         */

        public int CompareTo(NumberDistanceValue<N> other)
        {
            return (int)(this.DoubleValue() - other.DoubleValue());
        }

        /**
         * Returns a string representation of this NumberDistance.
         * 
         * @return the value of this NumberDistance.
         */

        public override String ToString()
        {
            return Value.ToString();
        }

        public override double ToDouble()
        {
            return DoubleValue();
        }
        ///**
        // * Returns the value of this NumberDistance.
        // * 
        // * @return the value of this NumberDistance
        // */
        //public abstract N GetValue();

        ///**
        // * Sets the value of this NumberDistance.
        // * 
        // * @param value the value to be set
        // */
        //public abstract void SetValue(N value);
        public abstract N Value { get; set; }
        /**
         * Get the value as double.
         * 
         * @return same result as getValue().doubleValue() but may be more efficient.
         */
        public abstract double DoubleValue();

      
        /**
         * Get the value as int.
         * 
         * @return same result as getValue().intValue() but may be more efficient.
         */
        public virtual int IntValue()
        {
            return (int)LongValue();
        }

        /**
         * Get the value as long.
         * 
         * @return same result as getValue().longValue() but may be more efficient.
         */
        public abstract long LongValue();

     
    }

}
