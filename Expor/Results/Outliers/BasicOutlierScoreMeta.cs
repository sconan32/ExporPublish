using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Log;

namespace Socona.Expor.Results.Outliers
{

    public class BasicOutlierScoreMeta : IOutlierScoreMeta
    {
        /**
         * Store the actual minimum
         */
        protected double actualMinimum = Double.NaN;

        /**
         * Store the actual maximum
         */
        protected double actualMaximum = Double.NaN;

        /**
         * Store the theoretical minimum
         */
        protected double theoreticalMinimum = Double.NaN;

        /**
         * Store the theoretical maximum
         */
        protected double theoreticalMaximum = Double.NaN;

        /**
         * Store the theoretical baseline
         */
        protected double theoreticalBaseline = Double.NaN;

        /**
         * Constructor with actual values only.
         * 
         * @param actualMinimum actual minimum
         * @param actualMaximum actual maximum
         */
        public BasicOutlierScoreMeta(double actualMinimum, double actualMaximum) :
            this(actualMinimum, actualMaximum, Double.NaN, Double.NaN, Double.NaN)
        {
        }

        /**
         * Constructor with all range values
         * 
         * @param actualMinimum actual minimum
         * @param actualMaximum actual maximum
         * @param theoreticalMinimum theoretical minimum
         * @param theoreticalMaximum theoretical maximum
         */
        public BasicOutlierScoreMeta(double actualMinimum, double actualMaximum, double theoreticalMinimum, double theoreticalMaximum) :
            this(actualMinimum, actualMaximum, theoreticalMinimum, theoreticalMaximum, Double.NaN)
        {
        }

        /**
         * Full constructor - all values.
         * 
         * @param actualMinimum actual minimum
         * @param actualMaximum actual maximum
         * @param theoreticalMinimum theoretical minimum
         * @param theoreticalMaximum theoretical maximum
         * @param theoreticalBaseline theoretical baseline
         */
        public BasicOutlierScoreMeta(double actualMinimum, double actualMaximum, double theoreticalMinimum, double theoreticalMaximum, double theoreticalBaseline) :
            base()
        {
            if (Double.IsNaN(actualMinimum) || Double.IsNaN(actualMaximum))
            {
                Logging.GetLogger(this.GetType()).Warning("Warning: Outlier Score meta initalized with NaN values: " +
                    actualMinimum + " - " + actualMaximum);
            }
            this.actualMinimum = actualMinimum;
            this.actualMaximum = actualMaximum;
            this.theoreticalMinimum = theoreticalMinimum;
            this.theoreticalMaximum = theoreticalMaximum;
            this.theoreticalBaseline = theoreticalBaseline;
        }


        public double GetActualMaximum()
        {
            return actualMaximum;
        }


        public double GetActualMinimum()
        {
            return actualMinimum;
        }


        public double GetTheoreticalBaseline()
        {
            return theoreticalBaseline;
        }


        public double GetTheoreticalMaximum()
        {
            return theoreticalMaximum;
        }


        public double GetTheoreticalMinimum()
        {
            return theoreticalMinimum;
        }


        public virtual double NormalizeScore(double value)
        {
            double center = 0.0;
            if (!Double.IsNaN(theoreticalBaseline) && !Double.IsInfinity(theoreticalBaseline))
            {
                center = theoreticalBaseline;
            }
            else if (!Double.IsNaN(theoreticalMinimum) && !Double.IsInfinity(theoreticalMinimum))
            {
                center = theoreticalMinimum;
            }
            else if (!Double.IsNaN(actualMinimum) && !Double.IsInfinity(actualMinimum))
            {
                center = actualMinimum;
            }
            if (value < center)
            {
                return 0.0;
            }
            double max = Double.NaN;
            if (!Double.IsNaN(theoreticalMaximum) && !Double.IsInfinity(theoreticalMaximum))
            {
                max = theoreticalMaximum;
            }
            else if (!Double.IsNaN(actualMaximum) && !Double.IsInfinity(actualMaximum))
            {
                max = actualMaximum;
            }
            if (!Double.IsNaN(max) && !Double.IsInfinity(max) && max >= center)
            {
                return (value - center) / (max - center);
            }
            return value - center;
        }

        /**
         * @param actualMinimum the actualMinimum to set
         */
        public void SetActualMinimum(double actualMinimum)
        {
            this.actualMinimum = actualMinimum;
        }

        /**
         * @param actualMaximum the actualMaximum to set
         */
        public void SetActualMaximum(double actualMaximum)
        {
            this.actualMaximum = actualMaximum;
        }


        public String LongName
        {
            get { return "Outlier Score Metadata"; }
        }


        public String ShortName
        {
            get { return "outlier-score-meta"; }
        }
    }
}
