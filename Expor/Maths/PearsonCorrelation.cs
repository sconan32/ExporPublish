using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths
{

    public class PearsonCorrelation
    {
        /**
         * Sum for XX
         */
        private double sumXX = 0;

        /**
         * Sum for YY
         */
        private double sumYY = 0;

        /**
         * Sum for XY
         */
        private double sumXY = 0;

        /**
         * Current mean for X
         */
        private double meanX = 0;

        /**
         * Current mean for Y
         */
        private double meanY = 0;

        /**
         * Weight sum
         */
        private double sumWe = 0;

        /**
         * Constructor.
         */
        public PearsonCorrelation()
        {

        }

        /**
         * Put a single value into the correlation statistic.
         * 
         * @param x Value in X
         * @param y Value in Y
         * @param w Weight
         */
        public void put(double x, double y, double w)
        {
            if (sumWe <= 0.0)
            {
                meanX = x;
                meanY = y;
                sumWe = w;
                return;
            }
            // Incremental update
            sumWe += w;
            // Delta to previous mean
            double deltaX = x - meanX;
            double deltaY = y - meanY;
            // Update means
            meanX += deltaX * w / sumWe;
            meanY += deltaY * w / sumWe;
            // Delta to new mean
            double neltaX = x - meanX;
            double neltaY = y - meanY;
            // Update
            sumXX += w * deltaX * neltaX;
            sumYY += w * deltaY * neltaY;
            // should equal weight * deltaY * neltaX!
            sumXY += w * deltaX * neltaY;
        }

        /**
         * Put a single value into the correlation statistic.
         * 
         * @param x Value in X
         * @param y Value in Y
         */
        public void put(double x, double y)
        {
            put(x, y, 1.0);
        }

        /**
         * Get the pearson correlation value.
         * 
         * @return Correlation value
         */
        public double GetCorrelation()
        {
            double popSdX = GetNaiveStddevX();
            double popSdY = GetNaiveStddevY();
            double covXY = GetNaiveCovariance();
            if (popSdX == 0 || popSdY == 0)
            {
                return 0;
            }
            return covXY / (popSdX * popSdY);
        }

        /**
         * Get the number of points the average is based on.
         * 
         * @return number of data points
         */
        public double GetCount()
        {
            return sumWe;
        }

        /**
         * Return mean of X
         * 
         * @return mean
         */
        public double GetMeanX()
        {
            return meanX;
        }

        /**
         * Return mean of Y
         * 
         * @return mean
         */
        public double GetMeanY()
        {
            return meanY;
        }

        /**
         * Get the covariance of X and Y (not taking sampling into account)
         * 
         * @return Covariance
         */
        public double GetNaiveCovariance()
        {
            return sumXY / sumWe;
        }

        /**
         * Get the covariance of X and Y (with sampling correction)
         * 
         * @return Covariance
         */
        public double GetSampleCovariance()
        {
            Debug.Assert(sumWe > 1);
            return sumXY / (sumWe - 1);
        }

        /**
         * Return the naive variance (not taking sampling into account)
         * 
         * Note: usually, you should be using {@link #getSampleVarianceX} instead!
         * 
         * @return variance
         */
        public double GetNaiveVarianceX()
        {
            return sumXX / sumWe;
        }

        /**
         * Return sample variance.
         * 
         * @return sample variance
         */
        public double GetSampleVarianceX()
        {
            Debug.Assert(sumWe > 1);
            return sumXX / (sumWe - 1);
        }

        /**
         * Return standard deviation using the non-sample variance
         * 
         * Note: usually, you should be using {@link #getSampleStddevX} instead!
         * 
         * @return stddev
         */
        public double GetNaiveStddevX()
        {
            return Math.Sqrt(GetNaiveVarianceX());
        }

        /**
         * Return standard deviation
         * 
         * @return stddev
         */
        public double GetSampleStddevX()
        {
            return Math.Sqrt(GetSampleVarianceX());
        }

        /**
         * Return the naive variance (not taking sampling into account)
         * 
         * Note: usually, you should be using {@link #getSampleVarianceY} instead!
         * 
         * @return variance
         */
        public double GetNaiveVarianceY()
        {
            return sumYY / sumWe;
        }

        /**
         * Return sample variance.
         * 
         * @return sample variance
         */
        public double GetSampleVarianceY()
        {
            Debug.Assert(sumWe > 1);
            return sumYY / (sumWe - 1);
        }

        /**
         * Return standard deviation using the non-sample variance
         * 
         * Note: usually, you should be using {@link #getSampleStddevY} instead!
         * 
         * @return stddev
         */
        public double GetNaiveStddevY()
        {
            return Math.Sqrt(GetNaiveVarianceY());
        }

        /**
         * Return standard deviation
         * 
         * @return stddev
         */
        public double GetSampleStddevY()
        {
            return Math.Sqrt(GetSampleVarianceY());
        }

        /**
         * Reset the value.
         */
        public void reset()
        {
            sumXX = 0;
            sumXY = 0;
            sumYY = 0;
            meanX = 0;
            meanY = 0;
            sumWe = 0;
        }
    }
}
