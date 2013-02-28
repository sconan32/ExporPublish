using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Documentation;
using Socona.Expor.Utilities.Exceptions;

namespace Socona.Expor.Maths
{

    [Reference(Authors = "B. P. Welford",
        Title = "Note on a method for calculating corrected sums of squares and products",
        BookTitle = "Technometrics 4(3)")]

    public class MeanVariance : Mean
    {
        /**
         * nVariance
         */
        protected double nvar = 0.0;

        /**
         * Empty constructor
         */
        public MeanVariance()
        {
            // nothing to do here, initialization done above.
        }

        /**
         * Constructor from other instance
         * 
         * @param other other instance to copy data from.
         */
        public MeanVariance(MeanVariance other)
        {
            this.mean = other.mean;
            this.nvar = other.nvar;
            this.wsum = other.wsum;
        }

        /**
         * Add a single value with weight 1.0
         * 
         * @param val Value
         */

        public override void Put(double val)
        {
            wsum += 1.0;
            double delta = val - mean;
            mean += delta / wsum;
            // The next line needs the *new* mean!
            nvar += delta * (val - mean);
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

        public override void Put(double val, double weight)
        {
            double nwsum = weight + wsum;
            double delta = val - mean;
            double rval = delta * weight / nwsum;
            mean += rval;
            // Use old and new weight sum here:
            nvar += wsum * delta * rval;
            wsum = nwsum;
        }

        /**
         * Join the data of another MeanVariance instance.
         * 
         * @param other Data to join with
         */

        public override void Put(Mean other)
        {
            if (other is MeanVariance)
            {
                MeanVariance mv = other as MeanVariance;
                double nwsum = mv.wsum + this.wsum;
                double delta = mv.mean - this.mean;
                double rval = delta * mv.wsum / nwsum;

                // this.mean += rval;
                // This supposedly is more numerically stable:
                this.mean = (this.wsum * this.mean + mv.wsum * mv.mean) / nwsum;
                this.nvar += ((MeanVariance)other).nvar + delta * this.wsum * rval;
                this.wsum = nwsum;
            }
            else
            {
                throw new AbortException("I cannot combine Mean and MeanVariance to a MeanVariance.");
            }
        }

        /**
         * Get the number of points the average is based on.
         * 
         * @return number of data points
         */

        public override double GetCount()
        {
            return wsum;
        }

        /**
         * Return mean
         * 
         * @return mean
         */

        public override double GetMean()
        {
            return mean;
        }

        /**
         * Return the naive variance (not taking sampling into account)
         * 
         * Note: usually, you should be using {@link #GetSampleVariance} instead!
         * 
         * @return variance
         */
        public double GetNaiveVariance()
        {
            return nvar / wsum;
        }

        /**
         * Return sample variance.
         * 
         * @return sample variance
         */
        public double GetSampleVariance()
        {
          //  Debug.Assert(wsum > 1, "Cannot compute a reasonable sample variance with weight <= 1.0!");
            return nvar / (wsum - 1);
        }

        /**
         * Return standard deviation using the non-sample variance
         * 
         * Note: usually, you should be using {@link #GetSampleStddev} instead!
         * 
         * @return stddev
         */
        public double GetNaiveStddev()
        {
            return Math.Sqrt(GetNaiveVariance());
        }

        /**
         * Return standard deviation
         * 
         * @return stddev
         */
        public double GetSampleStddev()
        {
            return Math.Sqrt(GetSampleVariance());
        }

        /**
         * Return the normalized value (centered at the mean, distance normalized by
         * standard deviation)
         * 
         * @param val original value
         * @return normalized value
         */
        public double NormalizeValue(double val)
        {
            return (val - GetMean()) / GetSampleStddev();
        }

        /**
         * Return the unnormalized value (centered at the mean, distance normalized by
         * standard deviation)
         * 
         * @param val normalized value
         * @return de-normalized value
         */
        public double DenormalizeValue(double val)
        {
            return (val * GetSampleStddev()) + GetMean();
        }

        /**
         * Create and initialize a new array of MeanVariance
         * 
         * @param dimensionality Dimensionality
         * @return New and initialized Array
         */
        public new  static MeanVariance[] NewArray(int dimensionality)
        {
            MeanVariance[] arr = new MeanVariance[dimensionality];
            for (int i = 0; i < dimensionality; i++)
            {
                arr[i] = new MeanVariance();
            }
            return arr;
        }


        public override String ToString()
        {
            return "MeanVariance(mean=" + GetMean() + ",var=" + GetSampleVariance() + ")";
        }


        public override void Reset()
        {
            base.Reset();
            nvar = 0;
        }
    }
}
