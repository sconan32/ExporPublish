using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Socona.Expor.Maths.Scales
{

    public class LinearScale
    {
        // at 31 scale steps, decrease resolution.
        private double ZOOMFACTOR = Math.Log10(31);

        /**
         * min value of the scale
         */
        private double min;

        /**
         * max value of the scale
         */
        private double max;

        /**
         * Scale resolution
         */
        private double res;

        /**
         * Scale resolution in log10.
         */
        private int log10res;

        /**
         * Scale delta := max - min
         */
        private double delta;

        /**
         * Constructor. Computes a scale covering the range of min-max with between 3
         * and 30 intervals, rounded to the appropriate number of digits.
         * 
         * @param min actual minimum in the data
         * @param max actual maximum in the data
         */
        public LinearScale(double min, double max)
        {
            if (max < min)
            {
                double tmp = max;
                max = min;
                min = tmp;
            }
            this.delta = max - min;
            if (this.delta <= Double.MinValue)
            {
                this.delta = 1.0;
            }
            log10res = (int)Math.Ceiling(Math.Log10(this.delta) - ZOOMFACTOR);
            res = Math.Pow(10, log10res);

            // round min and max according to the resolution counters
            this.min = Math.Floor(min / res) * res;
            this.max = Math.Ceiling(max / res) * res;
            if (this.min == this.max)
            {
                this.max = this.min + res;
            }
            // Update delta (note: updated min, max!)
            this.delta = this.max - this.min;
            if (this.delta <= Double.MinValue)
            {
                this.delta = 1.0;
            }

            // LoggingUtil.warning(min+"~"+this.min+" "+max+"~"+this.max+" % "+this.res+" "+this.delta);
        }

        /**
         * Get minimum value (scale, not data).
         * 
         * @return min
         */
        public double GetMin()
        {
            return min;
        }

        /**
         * Get maximum value (scale, not data).
         * 
         * @return max
         */
        public double GetMax()
        {
            return max;
        }

        /**
         * Get resolution (scale interval size)
         * 
         * @return scale interval size
         */
        public double GetRes()
        {
            return res;
        }

        /**
         * Get resolution (scale interval size)
         * 
         * @return scale interval size in logarithmic form
         */
        public double GetLog10Res()
        {
            return log10res;
        }

        /**
         * Covert a value to it's scale position
         * 
         * @param val data value
         * @return scale position in the interval [0:1]
         */
        public double GetScaled(double val)
        {
            return (val - min) / delta;
        }

        /**
         * Covert a scale position to the actual value
         * 
         * @param val scale position in the interval [0:1]
         * @return value on the original scale
         */
        public double GetUnscaled(double val)
        {
            return val * delta + min;
        }

        /**
         * Covert a relative value to it's scale position
         * 
         * @param val relative data value
         * @return relative scale position in the interval [0:1]
         */
        public double GetRelativeScaled(double val)
        {
            return val / delta;
        }

        /**
         * Covert a relative scale position to the actual value
         * 
         * @param val relative scale position in the interval [0:1]
         * @return relative value on the original scale
         */
        public double GetRelativeUnscaled(double val)
        {
            return val * delta;
        }

        /**
         * Covert a value to it's scale position
         * 
         * @param val data value
         * @param smin tarGet scale minimum
         * @param smax tarGet scale maximum
         * @return scale position in the interval [smin:smax]
         */
        public double GetScaled(double val, double smin, double smax)
        {
            return GetScaled(val) * (smax - smin) + smin;
        }

        /**
         * Covert a scale position to the actual value
         * 
         * @param val scale position in the interval [smin:smax]
         * @param smin tarGet scale minimum
         * @param smax tarGet scale maximum
         * @return value on the original scale
         */
        public double GetUnscaled(double val, double smin, double smax)
        {
            return GetUnscaled(val) * (smax - smin) + smin;
        }

        /**
         * Covert a relative value to it's scale position
         * 
         * @param val relative data value
         * @param smin tarGet scale minimum
         * @param smax tarGet scale maximum
         * @return relative scale position in the interval [smin:smax]
         */
        public double GetRelativeScaled(double val, double smax, double smin)
        {
            return GetRelativeScaled(val) * (smax - smin);
        }

        /**
         * Covert a relative scale position to the actual value
         * 
         * @param val relative scale position in the interval [smin:smax]
         * @param smin tarGet scale minimum
         * @param smax tarGet scale maximum
         * @return relative value on the original scale
         */
        public double GetRelativeUnscaled(double val, double smin, double smax)
        {
            return GetRelativeUnscaled(val) * (smax - smin);
        }

        /**
         * Format value according to the scales resolution (i.e. appropriate number of
         * digits)
         * 
         * @param val Value
         * @return formatted number
         */
        public String FormatValue(double val)
        {
            CultureInfo ci = new CultureInfo("en-US");
            ci.NumberFormat.NumberDecimalDigits = log10res;

            return val.ToString(ci);
        }
    }
}
