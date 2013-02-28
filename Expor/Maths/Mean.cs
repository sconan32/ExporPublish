using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths
{
    public class Mean
    {
        /**
         * Mean of values
         */
        protected double mean = 0.0;

        /**
         * Weight sum (number of samples)
         */
        protected double wsum = 0;

        /**
         * Empty constructor
         */
        public Mean()
        {
            // nothing to do here, initialization done above.
        }

        /**
         * Constructor from other instance
         * 
         * @param other other instance to copy data from.
         */
        public Mean(Mean other)
        {
            this.mean = other.mean;
            this.wsum = other.wsum;
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */
        public virtual void Put(double val)
        {
            wsum += 1.0;
            double delta = val - mean;
            mean += delta / wsum;
        }

        /**
         * Add data with a given weight.
         * 
         * See also: D.H.D. West<br />
         * Updating Mean and Variance Estimates: An Improved Method
         * 
         * @param val data
         * @param weight weight
         */
        public virtual void Put(double val, double weight)
        {
            double nwsum = weight + wsum;
            double delta = val - mean;
            double rval = delta * weight / nwsum;
            mean += rval;
            wsum = nwsum;
        }

        /**
         * Join the data of another MeanVariance instance.
         * 
         * @param other Data to join with
         */
        public virtual void Put(Mean other)
        {
            double nwsum = other.wsum + this.wsum;

            // this.mean += rval;
            // This supposedly is more numerically stable:
            this.mean = (this.wsum * this.mean + other.wsum * other.mean) / nwsum;
            this.wsum = nwsum;
        }

        /**
         * Get the number of points the average is based on.
         * 
         * @return number of data points
         */
        public virtual double GetCount()
        {
            return wsum;
        }

        /**
         * Return mean
         * 
         * @return mean
         */
        public virtual double GetMean()
        {
            return mean;
        }

        /**
         * Create and initialize a new array of MeanVariance
         * 
         * @param dimensionality Dimensionality
         * @return New and initialized Array
         */
        public static Mean[] NewArray(int dimensionality)
        {
            Mean[] arr = new Mean[dimensionality];
            for (int i = 0; i < dimensionality; i++)
            {
                arr[i] = new Mean();
            }
            return arr;
        }


        public override String ToString()
        {
            return "Mean(" + GetMean() + ")";
        }

        /**
         * Reset the value.
         */
        public virtual void Reset()
        {
            mean = 0;
            wsum = 0;
        }
    }
}
