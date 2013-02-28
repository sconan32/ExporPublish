using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Results.Outliers
{

    public class InvertedOutlierScoreMeta : BasicOutlierScoreMeta
    {
        /**
         * Constructor with all values.
         * 
         * @param actualMinimum Actual minimum in data
         * @param actualMaximum Actual maximum in data
         * @param theoreticalMinimum Theoretical minimum of algorithm
         * @param theoreticalMaximum Theoretical maximum of algorithm
         * @param theoreticalBaseline Theoretical Baseline
         */
        public InvertedOutlierScoreMeta(double actualMinimum, double actualMaximum,
            double theoreticalMinimum, double theoreticalMaximum, double theoreticalBaseline) :
            base(actualMinimum, actualMaximum, theoreticalMinimum, theoreticalMaximum, theoreticalBaseline)
        {
        }

        /**
         * Constructor with range values.
         * 
         * @param actualMinimum Actual minimum in data
         * @param actualMaximum Actual maximum in data
         * @param theoreticalMinimum Theoretical minimum of algorithm
         * @param theoreticalMaximum Theoretical maximum of algorithm
         */
        public InvertedOutlierScoreMeta(double actualMinimum, double actualMaximum,
            double theoreticalMinimum, double theoreticalMaximum) :
            base(actualMinimum, actualMaximum, theoreticalMinimum, theoreticalMaximum)
        {
        }

        /**
         * Constructor with actual range only.
         * 
         * @param actualMinimum Actual minimum in data
         * @param actualMaximum Actual maximum in data
         */
        public InvertedOutlierScoreMeta(double actualMinimum, double actualMaximum) :
            base(actualMinimum, actualMaximum)
        {
        }


        public override double NormalizeScore(double value)
        {
            double center = 0.0;
            if (!Double.IsNaN(theoreticalBaseline) && !Double.IsInfinity(theoreticalBaseline))
            {
                center = theoreticalBaseline;
            }
            else if (!Double.IsNaN(theoreticalMaximum) && !Double.IsInfinity(theoreticalMaximum))
            {
                center = theoreticalMaximum;
            }
            else if (!Double.IsNaN(actualMaximum) && !Double.IsInfinity(actualMaximum))
            {
                center = actualMaximum;
            }
            if (value > center)
            {
                return 0.0;
            }
            double min = Double.NaN;
            if (!Double.IsNaN(theoreticalMinimum) && !Double.IsInfinity(theoreticalMinimum))
            {
                min = theoreticalMinimum;
            }
            else if (!Double.IsNaN(actualMinimum) && !Double.IsInfinity(actualMinimum))
            {
                min = actualMinimum;
            }
            if (!Double.IsNaN(min) && !Double.IsInfinity(min) && min != center)
            {
                return (center - value) / (center - min);
            }
            return center - value;
        }
    }
}
