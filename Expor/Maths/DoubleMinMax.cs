using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Maths
{

    public class DoubleMinMax : DoubleDoublePair
    {
        /**
         * Constructor without starting values.
         * 
         * The minimum will be initialized to {@link Double#POSITIVE_INFINITY}.
         * 
         * The maximum will be initialized to {@link Double#NEGATIVE_INFINITY}.
         * 
         * So that the first data added will replace both.
         */
        public DoubleMinMax() :
            base(Double.PositiveInfinity, Double.NegativeInfinity)
        {
        }

        /**
         * Constructor with predefined minimum and maximum values.
         * 
         * @param min Minimum value
         * @param max Maximum value
         */
        public DoubleMinMax(double min, double max) :
            base(min, max)
        {
        }

        /**
         * Process a single double value.
         * 
         * If the new value is smaller than the current minimum, it will become the
         * new minimum.
         * 
         * If the new value is larger than the current maximum, it will become the new
         * maximum.
         * 
         * @param data New value
         */
        public void Put(double data)
        {
            this.first = Math.Min(this.first, data);
            this.second = Math.Max(this.second, data);
        }

        /**
         * Process a whole array of double values.
         * 
         * If any of the values is smaller than the current minimum, it will become
         * the new minimum.
         * 
         * If any of the values is larger than the current maximum, it will become the
         * new maximum.
         * 
         * @param data Data to process
         */
        public void Put(double[] data)
        {
            foreach (double value in data)
            {
                this.Put(value);
            }
        }

        /**
         * Process a whole collection of double values.
         * 
         * If any of the values is smaller than the current minimum, it will become
         * the new minimum.
         * 
         * If any of the values is larger than the current maximum, it will become the
         * new maximum.
         * 
         * @param data Data to process
         */
        public void Put(ICollection<Double> data)
        {
            foreach (Double value in data)
            {
                this.Put(value);
            }
        }

        /**
         * Get the current minimum.
         * 
         * @return current minimum.
         */
        public double GetMin()
        {
            return this.first;
        }

        /**
         * Get the current maximum.
         * 
         * @return current maximum.
         */
        public double GetMax()
        {
            return this.second;
        }

        /**
         * Return the difference between minimum and maximum.
         * 
         * @return Difference of current Minimum and Maximum.
         */
        public double GetDiff()
        {
            return this.GetMax() - this.GetMin();
        }

        /**
         * Test whether the result is defined.
         * 
         * @return true when at least one value has been added
         */
        public bool IsValid()
        {
            return (first <= second);
        }

        /**
         * Return minimum and maximum as array.
         * 
         * @return Minimum, Maximum
         */
        public double[] AsDoubleArray()
        {
            return new double[] { this.GetMin(), this.GetMax() };
        }

        /**
         * Generate a new array of initialized DoubleMinMax objects (with default
         * constructor)
         * 
         * @param size Array size
         * @return initialized array
         */
        public static DoubleMinMax[] NewArray(int size)
        {
            DoubleMinMax[] ret = new DoubleMinMax[size];
            for (int i = 0; i < size; i++)
            {
                ret[i] = new DoubleMinMax();
            }
            return ret;
        }

        /**
         * Reset statistics.
         */
        public void Reset()
        {
            first = Double.PositiveInfinity;
            second = Double.NegativeInfinity;
        }
    }

}
